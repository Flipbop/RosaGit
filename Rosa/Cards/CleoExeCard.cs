using Nanoray.PluginManager;
using Nickel;
using System.Collections.Generic;
using System.Reflection;

namespace Flipbop.Cleo;

internal sealed class CleoExeCard : Card, IRegisterable
{
	public static void Register(IPluginPackage<IModManifest> package, IModHelper helper)
	{
		helper.Content.Cards.RegisterCard("CleoExe", new()
		{
			CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				deck = Deck.colorless,
				rarity = Rarity.common,
				upgradesTo = [Upgrade.A, Upgrade.B]
			},
			Art = ModEntry.Instance.CleanSlateSprite.Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "CleoExe", "name"]).Localize
		});
	}

	private int GetChoiceCount()
		=> upgrade == Upgrade.B ? 3 : 2;

	public override CardData GetData(State state)
		=> new()
		{
			artTint = "ffffff",
			cost = upgrade == Upgrade.A ? 0 : 1,
			exhaust = true,
			description = ModEntry.Instance.Localizations.Localize(["card", "CleoExe", "description", upgrade.ToString()], new { Count = GetChoiceCount() })
		};

	public override List<CardAction> GetActions(State s, Combat c)
		=> [
			new ACardOffering
			{
				amount = GetChoiceCount(),
				limitDeck = ModEntry.Instance.CleoDeck.Deck,
				makeAllCardsTemporary = true,
				overrideUpgradeChances = false,
				canSkip = false,
				inCombat = true,
				discount = -1,
				dialogueSelector = $".summon{ModEntry.Instance.CleoDeck.UniqueName}"
			}
		];
}
