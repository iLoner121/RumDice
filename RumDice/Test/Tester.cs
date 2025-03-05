using RumDice.Core;
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
            t.Msg = "decklist 显示牌堆";
            t.MsgType=MsgType.Group;
            _handleGroupMessage(t);

            var c = new GroupMsg();
            c.Msg = ".CharacterList";
            c.MsgType=MsgType.Group;
            _handleGroupMessage(c);

            for(int i = 0; i < 10; i++) {
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
            var s = new GroupMsg();
            s.Msg = "丝莉洛德.CharacterChange";
            s.MsgType=MsgType.Group;
            _handleGroupMessage(s);
            var p = new GroupMsg();
            p.Msg = "故障机器人.CharacterChange";
            p.MsgType=MsgType.Group;
            _handleGroupMessage(p);
            var q = new GroupMsg();
            q.Msg = ".draw 蛞蝓猫的道具";
            q.MsgType=MsgType.Group;
            _handleGroupMessage(q);

            Task.Delay(100000).Wait();
            return;
        }
    }
}
