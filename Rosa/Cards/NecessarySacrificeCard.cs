using Nanoray.PluginManager;
using Nickel;
using System.Collections.Generic;
using System.Reflection;

namespace Flipbop.Cleo;

internal sealed class NecessarySacrificeCard : Card, IRegisterable
{
	public static void Register(IPluginPackage<IModManifest> package, IModHelper helper)
	{
		helper.Content.Cards.RegisterCard(MethodBase.GetCurrentMethod()!.DeclaringType!.Name, new()
		{
			CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				deck = ModEntry.Instance.CleoDeck.Deck,
				rarity = ModEntry.GetCardRarity(MethodBase.GetCurrentMethod()!.DeclaringType!),
				upgradesTo = [Upgrade.A, Upgrade.B]
			},
			Art = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("assets/Cards/NecessarySacrifice.png")).Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "NecessarySacrifice", "name"]).Localize
		});
	}

	public override CardData GetData(State state)
		=> new()
		{
			artTint = "996699",
			cost = upgrade == Upgrade.A ? 1 : 2,
			retain = upgrade == Upgrade.B,
			exhaust = true,
		};

	public override List<CardAction> GetActions(State s, Combat c)
		=>
		[
			ModEntry.Instance.KokoroApi.ActionCosts.MakeCostAction(ModEntry.Instance.KokoroApi.ActionCosts.MakeResourceCost(new ImpairedCost(), 2), new AStatus{targetPlayer = true, status = Status.perfectShield, statusAmount = 1}).AsCardAction,
		];
}
