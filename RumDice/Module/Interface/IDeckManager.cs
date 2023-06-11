using RumDice.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RumDice.Module {
    [MyClass]
    public interface IDeckManager {
        [IsPrivate]
        [PrefixMatch("draw")]
        string DrawCard(Post post);

        [MyService("draw:")]
        string DrawCard(string command);

        [Reply("deck")]
        [IsPrivate]
        [KeyWord("decklist 显示牌堆",isDivided:true)]
        [Reply("牌堆")]
        string DeckList(Post post);

        [Listen(AllType.Start)]
        void Initialize(Post post);
    }
}
