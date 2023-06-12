using RumDice.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RumDice.Test {
    public class Tester {
        Action<Post> _handlePrivateMessage;
        Action<Post> _handleGroupMessage;

        public void SetHandlePrivateMessage(Action<Post> handlePrivateMessage) {
            _handlePrivateMessage = handlePrivateMessage;
        }
        public void SetHandleGroupMessage(Action<Post> handleGroupMessage) {
            _handleGroupMessage = handleGroupMessage;
        }

        public async ValueTask RunTest() {
            var t = new GroupMsg();
            t.Msg = ".draw _抽牌压力测试";
            t.MsgType=MsgType.Group;
            _handleGroupMessage(t);

            for(int i = 0; i < 100; i++) {
                var post = new GroupMsg();
                
                switch (new Random().Next(1,6)) {
                    case 1:
                        post.Msg = "Echo echo测试信息" + i;
                        break;
                    case 2:
                        post.Msg = ".prefixt 测试" + i;
                        break;
                    case 3:
                        post.Msg = ".prefixtest 测试" + i;
                        break;
                    case 4:
                        post.Msg = "replytest test" + i;
                        break;
                    case 5:
                        post.Msg = "replytest";
                        break;
                    default:
                        break;

                }
                post.MsgType = MsgType.Group;
                _handleGroupMessage(post);
            }
            Task.Delay(100000).Wait();
            return;
        }
    }
}
