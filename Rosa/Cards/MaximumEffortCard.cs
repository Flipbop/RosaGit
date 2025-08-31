using Nanoray.PluginManager;
using Nickel;
using System.Collections.Generic;
using System.Reflection;

namespace Flipbop.Cleo;

internal sealed class MaximumEffortCard : Card, IRegisterable
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
			Art = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("assets/Cards/MaximumEffort.png")).Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "MaximumEffort", "name"]).Localize
		});
	}

	public override CardData GetData(State state)
		=> new()
		{
			artTint = "996699",
			cost = 2,
		};

	public override List<CardAction> GetActions(State s, Combat c)
		=> upgrade switch
		{
			Upgrade.A => [
				new AAttack{ damage = GetDmg(s, 3)},
				ModEntry.Instance.KokoroApi.ActionCosts.MakeCostAction(ModEntry.Instance.KokoroApi.ActionCosts.MakeResourceCost(new ImpairedCost(), 1), new AAttack{ damage = GetDmg(s, 3)}).AsCardAction,
				ModEntry.Instance.KokoroApi.ActionCosts.MakeCostAction(ModEntry.Instance.KokoroApi.ActionCosts.MakeResourceCost(new ImpairedCost(), 1), new AAttack{ damage = GetDmg(s, 3)}).AsCardAction,
			],
			Upgrade.B => [
				ModEntry.Instance.KokoroApi.ActionCosts.MakeCostAction(ModEntry.Instance.KokoroApi.ActionCosts.MakeResourceCost(new ImpairedCost(), 1), new AAttack{ damage = GetDmg(s, 4)}).AsCardAction,
				ModEntry.Instance.KokoroApi.ActionCosts.MakeCostAction(ModEntry.Instance.KokoroApi.ActionCosts.MakeResourceCost(new ImpairedCost(), 1), new AAttack{ damage = GetDmg(s, 3)}).AsCardAction,
				ModEntry.Instance.KokoroApi.ActionCosts.MakeCostAction(ModEntry.Instance.KokoroApi.ActionCosts.MakeResourceCost(new ImpairedCost(), 2), new AAttack{ damage = GetDmg(s, 5)}).AsCardAction,
			],
			_ => [
				ModEntry.Instance.KokoroApi.ActionCosts.MakeCostAction(ModEntry.Instance.KokoroApi.ActionCosts.MakeResourceCost(new ImpairedCost(), 1), new AAttack{ damage = GetDmg(s, 3)}).AsCardAction,
				ModEntry.Instance.KokoroApi.ActionCosts.MakeCostAction(ModEntry.Instance.KokoroApi.ActionCosts.MakeResourceCost(new ImpairedCost(), 1), new AAttack{ damage = GetDmg(s, 3)}).AsCardAction,
				ModEntry.Instance.KokoroApi.ActionCosts.MakeCostAction(ModEntry.Instance.KokoroApi.ActionCosts.MakeResourceCost(new ImpairedCost(), 1), new AAttack{ damage = GetDmg(s, 3)}).AsCardAction,
			]
		};
}
