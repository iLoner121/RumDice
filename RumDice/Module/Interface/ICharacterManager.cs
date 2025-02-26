using RumDice.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RumDice.Module{
    [MyClass]
    public interface ICharacterManager{
        [Reply(".CharacterList")]
        string ShowCharacterList(Post post);

        [KeyWord(".CharacterChange", IsSuffix=true)]
        string CharacterChange(Post post);

        [Listen(AllType.Start)]
        void Initialize(Post post);
    }
}