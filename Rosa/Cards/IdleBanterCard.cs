using Nanoray.PluginManager;
using Nickel;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

namespace Flipbop.Rosa;

internal sealed class IdleBanterCard : Card, IRegisterable
{
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
			Art = helper.Content.Sprites
				.RegisterSprite(package.PackageRoot.GetRelativeFile("assets/Cards/IdleBanter.png")).Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "IdleBanter", "name"]).Localize
		});
	}

	public override CardData GetData(State state)
		=> new()
		{
			artTint = "FFFFFF",
			cost = 2,

		};

	public override List<CardAction> GetActions(State s, Combat c)
		=> upgrade switch
		{
			Upgrade.A =>
			[
				new ADrawCard() {count = 1},
				new AVariableHint() {hand = true},
				new AStatus() {status = Status.tempShield, xHint = 1, statusAmount = c.hand.Count, targetPlayer = true},
			],
			Upgrade.B =>
			[
				new ADrawCard() {count = 3},
				new AVariableHint() {hand = true},
				new AStatus() {status = Status.tempShield, xHint = 1, statusAmount = c.hand.Count, targetPlayer = true},
				new AEndTurn()
			],
			_ =>
			[
				new ADrawCard() {count = 1},
				new AVariableHint() {hand = true},
				new AStatus() {status = Status.tempShield, xHint = 1, statusAmount = c.hand.Count, targetPlayer = true},
				new AEndTurn()
			]

		};
}