using Nanoray.PluginManager;
using Nickel;
using System.Collections.Generic;
using System.Reflection;

namespace Flipbop.Rosa;

internal sealed class CalmDownCard : Card, IRegisterable, IHasCustomCardTraits
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
			Art = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("assets/Cards/CalmDown.png")).Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "CalmDown", "name"]).Localize
		});
	}
	public IReadOnlySet<ICardTraitEntry> GetInnateTraits(State state)
		=> upgrade switch
		{
			_ => new HashSet<ICardTraitEntry>()
			{
				ModEntry.Instance.PatientTrait
			}
		};
	
	public override CardData GetData(State state)
		=> new()
		{
			artTint = "FFFFFF",
			cost = upgrade switch
			{
				Upgrade.A => 3,
				Upgrade.B => 5,
				_ => 4
			},
		};

	public override List<CardAction> GetActions(State s, Combat c)
		=> upgrade switch
		{
			Upgrade.B => [
				new AStatus { targetPlayer = false, status = ModEntry.Instance.KokoroApi.DriveStatus.Underdrive, statusAmount = 2 },
				new AStatus() {targetPlayer = true, status = Status.shield, statusAmount = 1}
			],
			_ => [
				new AStatus { targetPlayer = false, status = ModEntry.Instance.KokoroApi.DriveStatus.Underdrive, statusAmount = 1 },
				new AStatus() {targetPlayer = true, status = Status.shield, statusAmount = 1}
			]
		};
}
