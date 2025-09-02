using Nanoray.PluginManager;


using Nickel;
using System.Collections.Generic;
using System.Reflection;
using daisyowl.text;
using Shockah.Kokoro;

namespace Flipbop.Rosa;

internal sealed class BassBoosterCard : Card, IRegisterable
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
			Art = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("assets/Cards/BassBooster.png")).Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "BassBooster", "name"]).Localize
		});
	}

	public override CardData GetData(State state)
		=> new()
		{
			artTint = "FFFFFF",
			cost = upgrade == Upgrade.A ? 3 : 4,
			exhaust = true
		};

	public override List<CardAction> GetActions(State s, Combat c)
		=> upgrade switch
		{
			Upgrade.B => [
				new AStatus() {status = ModEntry.Instance.SuperBoostStatus.Status, statusAmount = 1, targetPlayer = false},
				new AStatus() {status = Status.boost, statusAmount = 2, targetPlayer = false}
			],
			_ => [
				new AStatus() {status = ModEntry.Instance.SuperBoostStatus.Status, statusAmount = 1, targetPlayer = false}
			],
		};

}

