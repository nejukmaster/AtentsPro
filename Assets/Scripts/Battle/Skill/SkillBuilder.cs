using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AtentsPro
{
    public static class SkillBuilder
    {
        public static Skill Build(string skillCode)
        {
            switch (skillCode)
            {
                #region DummyCharacter 스킬
                case "DMY_00":
                    return new DummyBasicAttack(DataSheetLoader.GetInstance().skillTable["DMY_00"].cost, DataSheetLoader.GetInstance().skillTable["DMY_00"].cooltime);
                case "DMY_01":
                    return new DummyFirstAttack(DataSheetLoader.GetInstance().skillTable["DMY_01"].cost, DataSheetLoader.GetInstance().skillTable["DMY_01"].cooltime);
                case "DMY_02":
                    return new DummySecondAttack(DataSheetLoader.GetInstance().skillTable["DMY_02"].cost, DataSheetLoader.GetInstance().skillTable["DMY_02"].cooltime);
                case "DMY_03":
                    return new DummyThirdAttack(DataSheetLoader.GetInstance().skillTable["DMY_03"].cost, DataSheetLoader.GetInstance().skillTable["DMY_03"].cooltime);
                #endregion
                #region Hunter 스킬
                case "HUN_00":
                    return new HunterBasicAttack(DataSheetLoader.GetInstance().skillTable["HUN_00"].cost, DataSheetLoader.GetInstance().skillTable["HUN_00"].cooltime);
                case "HUN_01":
                    return new HunterFirstAttack(DataSheetLoader.GetInstance().skillTable["HUN_01"].cost, DataSheetLoader.GetInstance().skillTable["HUN_01"].cooltime);
                case "HUN_02":
                    return new HunterSecondAttack(DataSheetLoader.GetInstance().skillTable["HUN_02"].cost, DataSheetLoader.GetInstance().skillTable["HUN_02"].cooltime);
                case "HUN_03":
                    return new HunterThirdAttack(DataSheetLoader.GetInstance().skillTable["HUN_03"].cost, DataSheetLoader.GetInstance().skillTable["HUN_03"].cooltime);
                #endregion
                #region Mizuki 스킬
                case "MZK_00":
                    return new MizukiBasicAttack(DataSheetLoader.GetInstance().skillTable["MZK_00"].cost, DataSheetLoader.GetInstance().skillTable["MZK_02"].cooltime);
                case "MZK_01":
                    return new MizukiFirstAttack(DataSheetLoader.GetInstance().skillTable["MZK_01"].cost, DataSheetLoader.GetInstance().skillTable["MZK_01"].cooltime);
                case "MZK_02":
                    return new MizukiSecondAttack(DataSheetLoader.GetInstance().skillTable["MZK_02"].cost, DataSheetLoader.GetInstance().skillTable["MZK_02"].cooltime);
                case "MZK_03":
                    return new MizukiThirdAttack(DataSheetLoader.GetInstance().skillTable["MZK_03"].cost, DataSheetLoader.GetInstance().skillTable["MZK_03"].cooltime);
                #endregion
                default:
                    return null;
            }
        }
    }
}
