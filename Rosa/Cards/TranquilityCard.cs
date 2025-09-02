using Nanoray.PluginManager;
using Nickel;
using System.Collections.Generic;
using System.Reflection;

namespace Flipbop.Rosa;

internal sealed class TranquilityCard : Card, IRegisterable
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
			Art = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("assets/Cards/Tranquility.png")).Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "Tranquility", "name"]).Localize
		});
	}

	public override CardData GetData(State state)
		=> new()
		{
			artTint = "FFFFFF",
			cost = 2,
			exhaust = true,
			retain = upgrade == Upgrade.A,
		};

	public override List<CardAction> GetActions(State s, Combat c)
		=> upgrade switch
		{
			
			Upgrade.B => [
				new AStatus {targetPlayer = false, status = ModEntry.Instance.SilenceStatus.Status, statusAmount = 10},
			],
			_ => [
				new AStatus {targetPlayer = false, status = ModEntry.Instance.SilenceStatus.Status, statusAmount = 7},
			]
		};
}
