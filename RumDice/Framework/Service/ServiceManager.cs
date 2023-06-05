using RumDice.Module;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RumDice.Framework {
    public class ServiceManager : IServiceManager {
        /// <summary>
        /// 服务类型
        /// </summary>
        enum ServiceType {
            SINGLETON,
            TRANSIENT,
            TIMED,
            COUNTED,
        }
        /// <summary>
        /// 单例实例列表
        /// </summary>
        Dictionary<string, object> _singletonTable;
        /// <summary>
        /// 记数实例列表
        /// </summary>
        Dictionary<string, List<object>> _countedTable;
        /// <summary>
        /// 计时实例列表
        /// </summary>
        Dictionary<string, Tuple<object, long>> _timedTable;
        /// <summary>
        /// 服务类型检查表
        /// </summary>
        Dictionary<string, ServiceType> _typeCheckTable;
        /// <summary>
        /// 服务生命周期检查表
        /// </summary>
        Dictionary<string, long> _lifetimeCheckTable;
        /// <summary>
        /// 服务最大记数检查表格
        /// </summary>
        Dictionary<string, int> _maxCheckTable;
        /// <summary>
        /// 接口与实现对照表
        /// </summary>
        Dictionary<Type, Type> _interfaceTable;

        Dictionary<string,Type> _classTable;

        readonly IRumLogger _logger;

        public ServiceManager(IRumLogger logger) {
            _singletonTable = new();
            _countedTable = new();
            _timedTable = new();
            _typeCheckTable = new();
            _lifetimeCheckTable = new();
            _maxCheckTable = new();
            _interfaceTable = new();
            _classTable = new();
            _logger = logger;
        }


        public async ValueTask Initialize() {
            await Task.Delay(1);
            var assemblyType = Assembly.GetAssembly(typeof(IExample)).ExportedTypes.ToList();
            foreach (var assembly in assemblyType) {
                // 只获取Class
                if (assembly.IsInterface) continue;

                Type[] interfaces = assembly.GetInterfaces();
                foreach (Type interf in interfaces) {
                    if (interf?.GetCustomAttribute(typeof(MyClassAttribute)) is not MyClassAttribute attribute) {
                        continue;
                    }
                    if (_interfaceTable.ContainsKey(interf)) {
                        _logger.Warn("ServiceManager", "+++类别添加失败：重复的接口或类别");
                        return;
                    }
                    SetTransient(interf,assembly);
                    _logger.Debug("ServiceManager", $"+++已将{interf.Name},{assembly.Name}加入对象管理器");
                }
            }

            foreach (var assembly in assemblyType) {
                // 只获取Class
                if (assembly.IsInterface) continue;

                if(assembly.GetCustomAttribute(typeof(MyStructAttribute)) is not MyStructAttribute attribute1) {
                    continue;
                }
                if (_classTable.ContainsKey(assembly.Name)) {
                    _logger.Warn("ServiceManager", "+++类别添加失败：重复的接口或类别");
                    continue;
                }
                _classTable.Add(assembly.Name, assembly);
                _logger.Debug("ServiceManager", $"+++已将{assembly.Name}加入对象管理器");
            }
        }

        /// <summary>
        /// 设置服务的统一部分
        /// </summary>
        /// <param name="type"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        bool SetService(Type interfaceType,Type serviceType, ServiceType t) {
            if (_typeCheckTable.ContainsKey(serviceType.Name)) {
                _logger.Warn("ServiceManager", "+++服务设置失败：已经设置过该服务");
                return false;
            }
            if (_interfaceTable.ContainsKey(interfaceType)) {
                _logger.Warn("ServiceManager", "+++服务设置失败：已经设置过相同接口的服务");
                return false;
            }
            _typeCheckTable.Add(serviceType.Name, t);
            _interfaceTable.Add(interfaceType, serviceType);
            return true;
        }
        /// <summary>
        /// 获取单例
        /// </summary>
        /// <param name="type">单例类别</param>
        /// <returns></returns>
        object GetSingleton(Type type) {
            if (_singletonTable.ContainsKey(type.Name)) {
                return _singletonTable[type.Name];
            }
            try {
                var instance = Activator.CreateInstance(type);
                _singletonTable.Add(type.Name, instance);
                return instance;
            }
            catch(Exception ex) {
                _logger.Error(ex, "+++生成对象失败");
            }
            return null;
        }
        /// <summary>
        /// 获取常新实例
        /// </summary>
        /// <param name="type">类别</param>
        /// <returns></returns>
        object GetTransient(Type type) {
            try {
                var instance = Activator.CreateInstance(type);
                return instance;
            }
            catch(Exception ex) {
                _logger.Error(ex, "+++生成对象失败");
            }
            return null;
        }

        public async ValueTask SetSingleton(Type it,Type st) {
            SetService(it,st, ServiceType.SINGLETON);
            return;
        }

        public async ValueTask SetTransient(Type it,Type st) {
            SetService(it,st, ServiceType.TRANSIENT);
            return;
        }

        public async ValueTask SetTimed(Type it,Type st, long time = 100) {
            SetService(it,st, ServiceType.TIMED);
            _lifetimeCheckTable.Add(st.Name, time);
            return;
        }

        public async ValueTask SetCounted(Type it,Type st, int max = 10) {
            SetService(it,st, ServiceType.COUNTED);
            _maxCheckTable.Add(st.Name, max);
            return;
        }

        public object GetService(Type type) {
            Type t = type;
            if (type.IsInterface) {
                if (!_interfaceTable.ContainsKey(type)) {
                    _logger.Warn("ServiceManager", $"+++服务获取失败：未设置过该服务{t.Name}");
                }
                t = _interfaceTable[type];
            }
            if (!_typeCheckTable.ContainsKey(t.Name)) {
                _logger.Warn("ServiceManager", $"+++服务获取失败：未设置过该服务{t.Name}");
                return null;
            }
            switch (_typeCheckTable[t.Name]) {
                case ServiceType.SINGLETON:
                    return GetSingleton(t);
                case ServiceType.TRANSIENT:
                    return GetTransient(t);
                case ServiceType.TIMED:
                    break;
                case ServiceType.COUNTED:
                    break;
                default:
                    break;
            }
            return null;
        }

        public object GetStruct(string name,out Type type) {
            type = null;
            if (!_classTable.ContainsKey(name)) {
                _logger.Warn("ServiceManager", "+++未找到该自定义存储类型");
                return null;
            }
            type= _classTable[name];
            return Activator.CreateInstance(_classTable[name]);
        }

        public List<object> GetServiceList(Type type) {
            throw new NotImplementedException();
        }

        public List<object> GetServiceList(List<Type> types) {
            throw new NotImplementedException();
        }
    }
}
