using Nanoray.PluginManager;
using Nickel;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

namespace Flipbop.Rosa;

internal sealed class GossipCard : Card, IRegisterable, IHasCustomCardTraits
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
			Art = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("assets/Cards/Gossip.png")).Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "Gossip", "name"]).Localize
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
			cost = 1,
			infinite = true,
		};

	public override List<CardAction> GetActions(State s, Combat c)
		=> upgrade switch
		{
			Upgrade.A =>
			[
				new AStatus { targetPlayer = true, status = Status.tempShield, statusAmount = 1},
				new ADrawCard() {count = 2}
			],
			Upgrade.B => [
				new AStatus { targetPlayer = true, status = Status.shield, statusAmount = 1},
				new ADrawCard() {count = 1}
			],
			_ => [
				new AStatus { targetPlayer = true, status = Status.tempShield, statusAmount = 1},
				new ADrawCard() {count = 1}
			]
			
		};
	
}
