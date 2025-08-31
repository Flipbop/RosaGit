using Nickel;
using System.Collections.Generic;

using HarmonyLib;


namespace Flipbop.Cleo;


internal static class DontLetCleoBecomeAnNPC
{
	public static void Apply(IHarmony harmony)
	{
		harmony.Patch(
			original: typeof(Events).GetMethod(nameof(Events.ShopFightBackOut))!,
			postfix: new HarmonyMethod(typeof(DontLetCleoBecomeAnNPC), nameof(ShopFightBackOutCleoEdition))
		);
		harmony.Patch(
			original: typeof(Events).GetMethod(nameof(Events.ShopSkipConfirm))!,
			postfix: new HarmonyMethod(typeof(DontLetCleoBecomeAnNPC), nameof(ShopSkipConfirmCleoEdition))
		);
		harmony.Patch(
			original: typeof(Shopkeep).GetMethod(nameof(Shopkeep.BuildShipForSelf))!,
			postfix: new HarmonyMethod(typeof(DontLetCleoBecomeAnNPC), nameof(NoneOfYourBusinessCleoEdition))
		);
	}

    private static void ShopSkipConfirmCleoEdition(State s, ref List<Choice> __result)
    {
		foreach (Character character in s.characters)  // There's no way to get Character from CleoCharacter or CleoDeck, so had to do it this way.
		{
			if (character.type == ModEntry.Instance.CleoCharacter.CharacterType)  // Checks if Cleo is in your crew, so it doesn't do anything if Cleo is not present
			{
				for (int x = 0; x < __result.Count; x++)
				{
					if (__result[x] is Choice c && c.key == "ShopSkipConfirm_No")
					{
						__result[x] = new Choice
						{
							label = "Nevermind",
							key = "Flipbop.Cleo::Shop.3"
						};
						return;
					}
				}
			}
		}
    }

    private static void ShopFightBackOutCleoEdition(State s, ref List<Choice> __result)
    {
		foreach (Character character in s.characters)
		{
			if (character.type == ModEntry.Instance.CleoCharacter.CharacterType)
			{
				for (int x = 0; x < __result.Count; x++)
				{
					if (__result[x] is Choice c && c.key == "ShopFightBackOut_No")
					{
						__result[x] = new Choice
						{
							label = "mmmmaybe not...",
							key = "Flipbop.Cleo::Shop.3"
						};
						return;
					}
				}
			}
		}
    }

    private static void NoneOfYourBusinessCleoEdition(State s, ref Shopkeep __instance)
    {
	    var cleoKiwi = ModEntry.Instance.KiwiCharacter.CharacterType;

	    foreach (Character crew in s.characters)
	    {
		    if (crew.type == ModEntry.Instance.CleoCharacter.CharacterType)
		    {
			    __instance.character = new Character() { type = cleoKiwi };
		    }
	    }
    }
}

internal sealed class EventDialogue : BaseDialogue
{

	public EventDialogue() : base(locale => ModEntry.Instance.Package.PackageRoot.GetRelativeFile($"i18n/dialogue-event-{locale}.json").OpenRead())
	{
		var cleoDeck = ModEntry.Instance.CleoDeck.Deck;
		var cleoType = ModEntry.Instance.CleoCharacter.CharacterType;
		var newNodes = new Dictionary<IReadOnlyList<string>, StoryNode>();
		var newHardcodedNodes = new Dictionary<IReadOnlyList<string>, StoryNode>();
		var saySwitchNodes = new Dictionary<IReadOnlyList<string>, Say>();
		
		ModEntry.Instance.Helper.Events.OnModLoadPhaseFinished += (_, phase) =>
		{
			if (phase != ModLoadPhase.AfterDbInit)
				return;			
			InjectStory(newNodes, newHardcodedNodes, saySwitchNodes, NodeType.@event);
		};
		ModEntry.Instance.Helper.Events.OnLoadStringsForLocale += (_, e) => 
		{
			foreach (KeyValuePair<string, StoryNode> node in DB.story.all)
			{
				if (node.Value.lookup?.Contains("shopBefore") == true && node.Value.allPresent?.Contains(cleoType) == false)
				{
					node.Value.nonePresent = [cleoType];
				}
			}

			#region Manual Overrides
			DB.story.all["ShopkeeperInfinite_Comp_Multi_0"].nonePresent = [cleoType];
			DB.story.all["ShopkeeperInfinite_Comp_Multi_1"].nonePresent = [cleoType];
			DB.story.all["ShopFightBackOut_No"].nonePresent = [cleoType];
			DB.story.all["ShopSkipConfirm_No"].nonePresent = [cleoType];
			DB.story.all["ShopAboutToDestroyYou"].nonePresent = [cleoType];
			DB.story.all["ShopHullWow_A"].nonePresent = [cleoType];
			DB.story.all["ShopHullWow_B"].nonePresent = [cleoType];
			DB.story.all["ShopHull_A"].nonePresent = [cleoType];
			DB.story.all["ShopHull_B"].nonePresent = [cleoType];
			DB.story.all["ShopHull_C"].nonePresent = [cleoType];
			DB.story.all["ShopNothingBadHull_A"].nonePresent = [cleoType];
			DB.story.all["ShopNothingBadHull_B"].nonePresent = [cleoType];
			DB.story.all["ShopNothing_1_Multi_0"].nonePresent = [cleoType];
			DB.story.all["ShopNothing_1_Multi_1"].nonePresent = [cleoType];
			DB.story.all["ShopNothing_1_Multi_2"].nonePresent = [cleoType];
			DB.story.all["ShopNothing_1_Multi_3"].nonePresent = [cleoType];
			DB.story.all["ShopNothing_1_Multi_4"].nonePresent = [cleoType];
			DB.story.all["ShopNothing_1_Multi_5"].nonePresent = [cleoType];
			DB.story.all["ShopNothing_1_Multi_6"].nonePresent = [cleoType];
			DB.story.all["ShopNothing_1_Multi_7"].nonePresent = [cleoType];
			DB.story.all["ShopNothing_1_Multi_8"].nonePresent = [cleoType];
			DB.story.all["ShopNothing_1_Multi_9"].nonePresent = [cleoType];
			DB.story.all["ShopNothing_1_Multi_10"].nonePresent = [cleoType];
			DB.story.all["ShopNothing_1_Multi_11"].nonePresent = [cleoType];
			DB.story.all["ShopNothing_1_Multi_12"].nonePresent = [cleoType];
			DB.story.all["ShopNothing_Confirm"].nonePresent = [cleoType];
			DB.story.all["ShopRemoveCard_Multi_0"].nonePresent = [cleoType];
			DB.story.all["ShopRemoveCard_Multi_1"].nonePresent = [cleoType];
			DB.story.all["ShopRemoveCard_Multi_2"].nonePresent = [cleoType];
			DB.story.all["ShopRemoveCard_Multi_3"].nonePresent = [cleoType];
			DB.story.all["ShopRemoveCard_Multi_4"].nonePresent = [cleoType];
			DB.story.all["ShopRemoveCard_Multi_5"].nonePresent = [cleoType];
			DB.story.all["ShopRemoveTwoCards_Multi_0"].nonePresent = [cleoType];
			DB.story.all["ShopRemoveTwoCards_Multi_1"].nonePresent = [cleoType];
			DB.story.all["ShopRemoveTwoCards_Multi_2"].nonePresent = [cleoType];
			DB.story.all["ShopUpgradeCard_Multi_0"].nonePresent = [cleoType];
			DB.story.all["ShopUpgradeCard_Multi_1"].nonePresent = [cleoType];
			DB.story.all["ShopUpgradeCard_Multi_2"].nonePresent = [cleoType];
			DB.story.all["ShopUpgradeCard_Multi_3"].nonePresent = [cleoType];
			DB.story.all["ShopUpgradeCard_Multi_4"].nonePresent = [cleoType];
			DB.story.all["ShopUpgradeCard_Multi_5"].nonePresent = [cleoType];
			DB.story.all["ShopUpgradeCard_Multi_6"].nonePresent = [cleoType];
			DB.story.all["EmptyShop"].nonePresent = [cleoType];
			#endregion
			InjectLocalizations(newNodes, newHardcodedNodes, saySwitchNodes, e);
		};
		
		
		newNodes[["Shop", "0"]] = new()
		{
			lookup = ["shopBefore"],
			bg = typeof(BGShop).Name,
			allPresent = [cleoType],
			priority = true,
			lines = [
				new Say { who = cleoType, loopTag = "neutral" },
				new Say { who = cleoType, loopTag = "neutral", flipped = true},
			],
			choiceFunc = "NewShop",
		};
		newNodes[["Shop", "1"]] = new()
		{
			lookup = ["shopBefore"],
			bg = typeof(BGShop).Name,
			allPresent = [cleoType],
			priority = true,
			lines = [
				new Say { who = cleoType, loopTag = "neutral" },
				new Say { who = cleoType, loopTag = "neutral", flipped = true},
			],
			choiceFunc = "NewShop",
		};

		#region Shop Dialouge Replacements
		//FIGHT ME
		newNodes[["Shop", "2"]] = new()
		{
			lookup = ["shopAboutToDestroyYou"],
			bg = typeof(BGShop).Name,
			allPresent = [cleoType],
			priority = true,
			lines = [
				new Say { who = cleoType, loopTag = "squint", flipped = true},
				new Say { who = cleoType, loopTag = "explain", flipped = true},
			],
			choiceFunc = "ShopFightBackOut",
		};
		//maaaaaaaybe not
		newNodes[["Shop", "3"]] = new()
		{
			lookup = ["shopFightBackOut_No"],
			bg = typeof(BGShop).Name,
			allPresent = [cleoType],
			priority = true,
			lines = [
				new Say { who = cleoType, loopTag = "neutral", flipped = true},
				new Say { who = cleoType, loopTag = "neutral", flipped = true},
			],
		};
		//Heal WOW
		newNodes[["Shop", "4"]] = new()
		{
			lookup = ["shopHeal"],
			bg = typeof(BGShop).Name,
			allPresent = [cleoType],
			priority = true,
			maxHullPercent = 0.5,
			lines = [
				new Say { who = cleoType, loopTag = "squint", flipped = true},
			],
		};
		//Heal
		newNodes[["Shop", "5"]] = new()
		{
			lookup = ["shopHeal"],
			bg = typeof(BGShop).Name,
			allPresent = [cleoType],
			priority = true,
			lines = [
				new Say { who = cleoType, loopTag = "neutral", flipped = true},
			],
		};
		//Skip Repairs
		newNodes[["Shop", "6"]] = new()
		{
			lookup = ["shopSkip"],
			bg = typeof(BGShop).Name,
			allPresent = [cleoType],
			priority = true,
			maxHullPercent = 0.5,
			lines = [
				new Say { who = cleoType, loopTag = "squint", flipped = true},
			],
		};
		//Skip 
		newNodes[["Shop", "7"]] = new()
		{
			lookup = ["shopSkip"],
			bg = typeof(BGShop).Name,
			allPresent = [cleoType],
			priority = true,
			lines = [
				new Say { who = cleoType, loopTag = "squint", flipped = true},
			],
		};
		//Skip Confirm?
		newNodes[["Shop", "8"]] = new()
		{
			lookup = ["shopSkip_Confirm"],
			bg = typeof(BGShop).Name,
			allPresent = [cleoType],
			priority = true,
			lines = [
				new Say { who = cleoType, loopTag = "squint", flipped = true},
			],
			choiceFunc = "ShopSkipConfirm"
		};
		//Card Removed
		newNodes[["Shop", "9"]] = new()
		{
			lookup = ["shopRemoveCard"],
			bg = typeof(BGShop).Name,
			allPresent = [cleoType],
			priority = true,
			lines = [
				new Say { who = cleoType, loopTag = "neutral", flipped = true},
			],
		};
		//2 Cards Removed
		newNodes[["Shop", "10"]] = new()
		{
			lookup = ["shopRemoveTwoCards"],
			bg = typeof(BGShop).Name,
			allPresent = [cleoType],
			priority = true,
			lines = [
				new Say { who = cleoType, loopTag = "neutral", flipped = true},
			],
		};
		//Skip Back Out
		newNodes[["Shop", "3"]] = new()
		{
			lookup = ["shopSkipConfirm_No"],
			bg = typeof(BGShop).Name,
			allPresent = [cleoType],
			priority = true,
			lines = [
				new Say { who = cleoType, loopTag = "neutral", flipped = true},
			],
			choiceFunc = "NewShop"
		};
		//Upgrade Card A
		newNodes[["Shop", "11"]] = new()
		{
			lookup = ["shopUpgradeCard"],
			bg = typeof(BGShop).Name,
			allPresent = [cleoType],
			priority = true,
			lines = [
				new Say { who = cleoType, loopTag = "neutral", flipped = true},
			],
		};
		//Upgrade Card B
		newNodes[["Shop", "12"]] = new()
		{
			lookup = ["shopUpgradeCard"],
			bg = typeof(BGShop).Name,
			allPresent = [cleoType],
			priority = true,
			lines = [
				new Say { who = cleoType, loopTag = "neutral", flipped = true},
			],
		};
		//Empty Shop
		newNodes[["Shop", "21"]] = new()
		{
			hasArtifacts = ["BrokenGlasses"],
			lookup = ["shopBefore"],
			bg = typeof(BGShopEmpty).Name,
			allPresent = [cleoType],
			priority = true,
			lines = [
				new Say { who = "comp", loopTag = "grumpy" },
				new Say { who = "comp", loopTag = "grumpy"},
				new Say { who = "comp", loopTag = "grumpy"},
				new Say { who = cleoType, loopTag = "squint"},
				new Say { who = "comp", loopTag = "grumpy"},
			],
		};
		#endregion

		#region Vanilla Yapping
		newNodes[["Shop", "13"]] = new()
		{
			lookup = ["shopBefore"],
			bg = typeof(BGShop).Name,
			allPresent = [cleoType],
			priority = true,
			lines = [
				new Say { who = "comp", loopTag = "neutral" },
				new Say { who = cleoType, loopTag = "neutral"},
				new Say { who = cleoType, loopTag = "neutral", flipped = true},
			],
			choiceFunc = "NewShop",
		};
		newNodes[["Shop", "14"]] = new()
		{
			lookup = ["shopBefore"],
			bg = typeof(BGShop).Name,
			allPresent = [cleoType, "dizzy"],
			priority = true,
			lines = [
				new Say { who = "dizzy", loopTag = "squint" },
				new Say { who = cleoType, loopTag = "explain"},
				new Say { who = cleoType, loopTag = "neutral", flipped = true},
			],
			choiceFunc = "NewShop",
		};
		newNodes[["Shop", "15"]] = new()
		{
			lookup = ["shopBefore"],
			bg = typeof(BGShop).Name,
			allPresent = [cleoType, "riggs"],
			priority = true,
			lines = [
				new Say { who = "riggs", loopTag = "neutral" },
				new Say { who = cleoType, loopTag = "neutral"},
				new Say { who = cleoType, loopTag = "neutral", flipped = true},
			],
			choiceFunc = "NewShop",
		};
		newNodes[["Shop", "16"]] = new()
		{
			lookup = ["shopBefore"],
			bg = typeof(BGShop).Name,
			allPresent = [cleoType, "peri"],
			priority = true,
			lines = [
				new Say { who = "peri", loopTag = "neutral" },
				new Say { who = cleoType, loopTag = "neutral"},
				new Say { who = cleoType, loopTag = "neutral", flipped = true},
			],
			choiceFunc = "NewShop",
		};
		newNodes[["Shop", "17"]] = new()
		{
			lookup = ["shopBefore"],
			bg = typeof(BGShop).Name,
			allPresent = [cleoType, "goat"],
			priority = true,
			lines = [
				new Say { who = "goat", loopTag = "neutral" },
				new Say { who = cleoType, loopTag = "neutral"},
				new Say { who = "goat", loopTag = "neutral" },
				new Say { who = cleoType, loopTag = "neutral", flipped = true},
			],
			choiceFunc = "NewShop",
		};
		newNodes[["Shop", "18"]] = new()
		{
			lookup = ["shopBefore"],
			bg = typeof(BGShop).Name,
			allPresent = [cleoType, "eunice"],
			priority = true,
			lines = [
				new Say { who = cleoType, loopTag = "neutral" },
				new Say { who = "eunice", loopTag = "blush"},
				new Say { who = cleoType, loopTag = "neutral", flipped = true},
			],
			choiceFunc = "NewShop",
		};
		newNodes[["Shop", "19"]] = new()
		{
			lookup = ["shopBefore"],
			bg = typeof(BGShop).Name,
			allPresent = [cleoType, "hacker"],
			priority = true,
			lines = [
				new Say { who = "hacker", loopTag = "squint" },
				new Say { who = cleoType, loopTag = "explain"},
				new Say { who = cleoType, loopTag = "neutral", flipped = true},
			],
			choiceFunc = "NewShop",
		};
		newNodes[["Shop", "20"]] = new()
		{
			lookup = ["shopBefore"],
			bg = typeof(BGShop).Name,
			allPresent = [cleoType, "shard"],
			priority = true,
			lines = [
				new Say { who = "shard", loopTag = "neutral" },
				new Say { who = cleoType, loopTag = "neutral"},
				new Say { who = cleoType, loopTag = "neutral", flipped = true},
			],
			choiceFunc = "NewShop",
		};
		#endregion
		
		newHardcodedNodes[["LoseCharacterCard_{{CharacterType}}"]] = new()
		{
			oncePerRun = true,
			bg = typeof(BGSupernova).Name,
			allPresent = [cleoType],
			lines = [
				new Say { who = cleoType, loopTag = "neutral" },
			],
		};
		newHardcodedNodes[["CrystallizedFriendEvent_{{CharacterType}}", "0"]] = new()
		{
			oncePerRun = true,
			bg = typeof(BGCrystalizedFriend).Name,
			allPresent = [cleoType],
			lines = [
				new Wait() { secs = 1.5 },
				new Say { who = cleoType, loopTag = "neutral" },
			],
		};
		newHardcodedNodes[["ChoiceCardRewardOfYourColorChoice_{{CharacterType}}"]] = new()
		{
			oncePerRun = true,
			bg = typeof(BGBootSequence).Name,
			allPresent = [cleoType],
			lines = [
				new Say { who = cleoType, loopTag = "squint" },
				new Say { who = "comp", loopTag = "neutral" },
			],
		};

		saySwitchNodes[["GrandmaShop"]] = new()
		{
			who = cleoType,
			loopTag = "neutral"
		};
		saySwitchNodes[["LoseCharacterCard"]] = new()
		{
			who = cleoType,
			loopTag = "nervous"
		};
		saySwitchNodes[["CrystallizedFriendEvent"]] = new()
		{
			who = cleoType,
			loopTag = "neutral"
		};
		saySwitchNodes[["ShopKeepBattleInsult"]] = new()
		{
			who = cleoType,
			loopTag = "nervous"
		};
		saySwitchNodes[["DraculaTime"]] = new()
		{
			who = cleoType,
			loopTag = "squint"
		};
		saySwitchNodes[["Soggins_Infinite"]] = new()
		{
			who = cleoType,
			loopTag = "neutral"
		};
		saySwitchNodes[["Soggins_Missile_Shout_1"]] = new()
		{
			who = cleoType,
			loopTag = "neutral"
		};
		saySwitchNodes[["SogginsEscapeIntent_1"]] = new()
		{
			who = cleoType,
			loopTag = "neutral"
		};
		saySwitchNodes[["SogginsEscape_1"]] = new()
		{
			who = cleoType,
			loopTag = "neutral"
		};
	}
}
