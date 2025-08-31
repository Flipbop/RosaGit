using Nanoray.PluginManager;
using Nickel;
using System.Collections.Generic;
using System.Reflection;

namespace Flipbop.Cleo;

internal sealed class TurtleShotCard : Card, IRegisterable
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
			Art = ModEntry.Instance.TurtleShotSprite.Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "TurtleShot", "name"]).Localize
		});
	}

	public override CardData GetData(State state)
		=> new()
		{
			artTint = "996699",
			cost = 2,
			exhaust = upgrade == Upgrade.B ? true : false,
		};

	public override List<CardAction> GetActions(State s, Combat c)
		=> upgrade switch
		{
			Upgrade.A =>
			[
				new AAttack { damage = GetDmg(s, 2) },
				new AStatus { targetPlayer = true, status = Status.shield, statusAmount = 1 },
				new AStatus { targetPlayer = true, status = Status.tempShield, statusAmount = 2 },
			],
			Upgrade.B => [
				new AAttack { damage = GetDmg(s, 3) },
				new AStatus { targetPlayer = true, status = Status.tempShield, statusAmount = 3 },
			],
			_ => [
				new AAttack { damage = GetDmg(s, 2) },
				new AStatus { targetPlayer = true, status = Status.tempShield, statusAmount = 2 },
			]
		};
}
