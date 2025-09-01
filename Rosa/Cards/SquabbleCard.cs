using Nanoray.PluginManager;
using Nickel;
using System.Collections.Generic;
using System.Reflection;

namespace Flipbop.Rosa;

internal sealed class SquabbleCard : Card, IRegisterable, IHasCustomCardTraits
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
			Art = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("assets/Cards/Squabble.png")).Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "Squabble", "name"]).Localize
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
			cost = upgrade == Upgrade.B ? 3 : 2,
		};

	public override List<CardAction> GetActions(State s, Combat c)
		=> upgrade switch
		{
			Upgrade.B => [
				new AStatus() {targetPlayer = false, status = ModEntry.Instance.FrazzleStatus.Status, statusAmount = 2},
				new ADrawCard() {count = 3}
			],
			Upgrade.A => [
				new AStatus() {targetPlayer = false, status = ModEntry.Instance.FrazzleStatus.Status, statusAmount = 1},
				new ADrawCard() {count = 4}			
			],
			_ => [
				new AStatus() {targetPlayer = false, status = ModEntry.Instance.FrazzleStatus.Status, statusAmount = 1},
				new ADrawCard() {count = 3}
			]
		};
}
