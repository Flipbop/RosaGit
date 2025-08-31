using Nanoray.PluginManager;
using Nickel;
using System.Collections.Generic;
using System.Reflection;

namespace Flipbop.Cleo;

internal sealed class ChoicesCard : Card, IRegisterable
{
	private static Spr _bSprite;
	public static void Register(IPluginPackage<IModManifest> package, IModHelper helper)
	{
		_bSprite = helper.Content.Sprites
			.RegisterSprite(package.PackageRoot.GetRelativeFile("assets/Cards/ChoicesB.png")).Sprite;
		helper.Content.Cards.RegisterCard(MethodBase.GetCurrentMethod()!.DeclaringType!.Name, new()
		{
			CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				deck = ModEntry.Instance.CleoDeck.Deck,
				rarity = ModEntry.GetCardRarity(MethodBase.GetCurrentMethod()!.DeclaringType!),
				upgradesTo = [Upgrade.A, Upgrade.B]
			},
			Art = _bSprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "Choices", "name"]).Localize
		});
	}

	public override CardData GetData(State state)
		=> new()
		{
			artTint = "996699",
			cost = upgrade == Upgrade.A? 0 : 1,
			floppable = upgrade != Upgrade.B, 
			art = upgrade switch {
				Upgrade.B => _bSprite,
				_ => flipped ? StableSpr.cards_Adaptability_Bottom : StableSpr.cards_Adaptability_Top,
			}
		};

	public override List<CardAction> GetActions(State s, Combat c)
		=> upgrade switch
		{
			Upgrade.B => [
				new AImproveA { Amount = 1 },
				new AImproveB { Amount = 1 },
			],
			_ => [
				new AImproveA { Amount = 1, disabled = flipped},
				new ADummyAction(),
				new AImproveB { Amount = 1, disabled = !flipped }
			]
		};
}
