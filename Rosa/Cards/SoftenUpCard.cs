using Nanoray.PluginManager;
using Nickel;
using System.Collections.Generic;
using System.Reflection;

namespace Flipbop.Rosa;

internal sealed class SoftenUpCard : Card, IRegisterable, IHasCustomCardTraits
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
			Art = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("assets/Cards/SoftenUp.png")).Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "SoftenUp", "name"]).Localize
		});
	}

	public IReadOnlySet<ICardTraitEntry> GetInnateTraits(State state)
	{
		this.SetIsPatient(true);
		HashSet<ICardTraitEntry> cardTraitEntries = new HashSet<ICardTraitEntry>();
		if (upgrade == Upgrade.B)
		{
			cardTraitEntries.Add(ModEntry.Instance.PatientTrait);
		}
		return cardTraitEntries;
	}
	public override CardData GetData(State state)
		=> new()
		{
			artTint = "FFFFFF",
			cost = upgrade == Upgrade.B? 3: 2,
			retain = true,
		};

	public override List<CardAction> GetActions(State s, Combat c)
		=> upgrade switch
		{
			Upgrade.A => [
				new AStatus() {status = ModEntry.Instance.KokoroApi.DriveStatus.Underdrive, statusAmount = 1, targetPlayer = false},
				new AStatus() {status = ModEntry.Instance.SilenceStatus.Status, statusAmount = 1, targetPlayer = false},
			],
			_ => [
				new AStatus() {status = ModEntry.Instance.KokoroApi.DriveStatus.Underdrive, statusAmount = 1, targetPlayer = false},
			]
		};
}
