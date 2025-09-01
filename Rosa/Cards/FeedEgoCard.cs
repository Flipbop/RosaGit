using Nanoray.PluginManager;
using Nickel;
using System.Collections.Generic;
using System.Reflection;

namespace Flipbop.Rosa;

internal sealed class FeedEgoCard : Card, IRegisterable
{
	private static Spr _bSprite;
	public static void Register(IPluginPackage<IModManifest> package, IModHelper helper)
	{
		helper.Content.Cards.RegisterCard(MethodBase.GetCurrentMethod()!.DeclaringType!.Name, new()
		{
			CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				deck = ModEntry.Instance.RosaDeck.Deck,
				rarity = ModEntry.GetCardRarity(MethodBase.GetCurrentMethod()!.DeclaringType!),
				upgradesTo = [Upgrade.A, Upgrade.B]
			},
			Art =helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("assets/Cards/FeedEgo.png")).Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "FeedEgo", "name"]).Localize
		});
	}

	public override CardData GetData(State state)
		=> new()
		{
			artTint = "FFFFFF",
			cost = upgrade == Upgrade.A? 0 : 1,
		};

	public override List<CardAction> GetActions(State s, Combat c)
		=> upgrade switch
		{
			Upgrade.B => [
				new AStatus() {targetPlayer = false, status = Status.boost, statusAmount = 2}
			],
			_ => [
				new AStatus() {targetPlayer = false, status = Status.boost, statusAmount = 1}
			]
		};
}
