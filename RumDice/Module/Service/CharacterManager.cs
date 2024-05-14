using CSScriptLib;
using RumDice.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RumDice.Module{
    public class CharacterManager: ICharacterManager{
        private Character? character;

        string ShowCharacterList(Post post){
            if (DataCenter.Instance.GetObj("\\Character"))
        }

        string CharacterChange(Post post);
    }
}