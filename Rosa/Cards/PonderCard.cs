using Nanoray.PluginManager;
using Nickel;
using System.Collections.Generic;
using System.Reflection;

namespace Flipbop.Rosa;

internal sealed class PonderCard : Card, IRegisterable, IHasCustomCardTraits
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
			Art = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("assets/Cards/Ponder.png")).Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "Ponder", "name"]).Localize
		});
	}

	public IReadOnlySet<ICardTraitEntry> GetInnateTraits(State state)
	{
		this.SetIsPatient(true);
		HashSet<ICardTraitEntry> cardTraitEntries = new HashSet<ICardTraitEntry>()
		{
			ModEntry.Instance.PatientTrait
		};
		return cardTraitEntries;
	}
	public override CardData GetData(State state)
		=> new()
		{
			artTint = "FFFFFF",
			cost = upgrade == Upgrade.B? 2 : 1,
		};

	public override List<CardAction> GetActions(State s, Combat c)
		=> upgrade switch
		{
			Upgrade.A => [
				new ADrawCard() {count = 3}
			],
			Upgrade.B => [
				new ADrawCard() {count = 5}
			],
			_ => [
				new ADrawCard() {count = 2}
			]
		};
}
