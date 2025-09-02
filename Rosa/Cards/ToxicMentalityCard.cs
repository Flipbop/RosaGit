using Nanoray.PluginManager;
using Nickel;
using System.Collections.Generic;
using System.Reflection;
using daisyowl.text;
using Shockah.Kokoro;


namespace Flipbop.Rosa;

internal sealed class ToxicMentalityCard : Card, IRegisterable, IHasCustomCardTraits
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
				.RegisterSprite(package.PackageRoot.GetRelativeFile("assets/Cards/ToxicMentality.png")).Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "ToxicMentality", "name"]).Localize
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
			cost = upgrade switch
			{
				Upgrade.A => 2,
				Upgrade.B => 4,
				_=> 3,
			},
			exhaust = upgrade != Upgrade.B,
		};

	public override List<CardAction> GetActions(State s, Combat c)
		=> upgrade switch
		{
			
			_ => [
				new AStatus() {status = Status.corrode, statusAmount = 1, targetPlayer = false}
			]
		};
	
}

