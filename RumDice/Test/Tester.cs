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
            var post=new PrivateMessage();
            post.Msg = "echo xxfadafdfas";
            _handlePrivateMessage(post);

            return;
        }
    }
}
