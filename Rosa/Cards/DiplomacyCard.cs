using FSPRO;
using Nanoray.PluginManager;
using Nickel;
using System.Collections.Generic;
using System.Reflection;

namespace Flipbop.Rosa;

internal sealed class DiplomacyCard : Card, IRegisterable
{
	private static Spr _flippedSprite;
	private static Spr _floppedSprite;
	private static Spr _aSprite;
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
			Art = helper.Content.Sprites
				.RegisterSprite(package.PackageRoot.GetRelativeFile("assets/Cards/ResourceSwapB.png")).Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "Diplomacy", "name"]).Localize
		});
		
	}

	public override CardData GetData(State state)
		=> new()
		{
			artTint = "FFFFFF",
			cost = upgrade == Upgrade.B? 2:1,
			exhaust = upgrade == Upgrade.B,
		};

	public override List<CardAction> GetActions(State s, Combat c)
		=> upgrade switch
		{
			Upgrade.A => [
				new AStatus() {status = ModEntry.Instance.KokoroApi.DriveStatus.Underdrive, statusAmount = 2, targetPlayer = false},
				new AStatus() {status = ModEntry.Instance.SympathyStatus.Status, statusAmount = 1, targetPlayer = false}
			],
			_ => [
				new AStatus() {status = ModEntry.Instance.KokoroApi.DriveStatus.Underdrive, statusAmount = 1, targetPlayer = false},
				new AStatus() {status = ModEntry.Instance.SympathyStatus.Status, statusAmount = 1, targetPlayer = false}
			]
		};
}