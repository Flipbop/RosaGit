using Nanoray.PluginManager;
using Nickel;
using System.Collections.Generic;
using System.Reflection;

namespace Flipbop.Rosa;

internal sealed class MuteCard : Card, IRegisterable
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
			Art = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("assets/Cards/Mute.png")).Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "Mute", "name"]).Localize
		});
	}

	public override CardData GetData(State state)
		=> new()
		{
			artTint = "FFFFFF",
			cost = 1,
			retain = upgrade == Upgrade.A,
		};

	public override List<CardAction> GetActions(State s, Combat c)
		=> upgrade switch
		{
			Upgrade.B =>
			[
				new AStatus() {status = ModEntry.Instance.SilenceStatus.Status, statusAmount = 1, targetPlayer = false},
				new AStatus() {status = Status.shield, statusAmount = 1, targetPlayer = true}
			],
			_ => [
				new AStatus() {status = ModEntry.Instance.SilenceStatus.Status, statusAmount = 1, targetPlayer = false}
			]
		};
}
