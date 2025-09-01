using Nanoray.PluginManager;
using Nickel;
using System.Collections.Generic;
using System.Reflection;

namespace Flipbop.Rosa;

internal sealed class KnockEmDownCard : Card, IRegisterable
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
				upgradesTo = [Upgrade.A, Upgrade.B],
				dontOffer = true,
			},
			Art = helper.Content.Sprites
				.RegisterSprite(package.PackageRoot.GetRelativeFile("assets/Cards/KnockEmDown.png")).Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "KnockEmDown", "name"]).Localize
		});
	}

	public override CardData GetData(State state)
		=> new()
		{
			artTint = "FFFFFF",
			cost = upgrade == Upgrade.A ? 1 : 2,
			singleUse = true,
			description =
				ModEntry.Instance.Localizations.Localize(["card", "KnockEmDown", "description", upgrade.ToString()])
		};

	public override List<CardAction> GetActions(State s, Combat c)
		=> upgrade switch
		{
			Upgrade.A =>
			[
				new AStatus { targetPlayer = false, status = ModEntry.Instance.KokoroApi.DriveStatus.Underdrive, statusAmount = 1 },
				new AAddCard { amount = 1, card = new SetEmUpCard() { upgrade = Upgrade.A }, destination = CardDestination.Discard },
			],
			Upgrade.B =>
			[
				new AStatus { targetPlayer = false, status = ModEntry.Instance.KokoroApi.DriveStatus.Underdrive, statusAmount = 1 },
				new AAddCard { amount = 1, card = new SetEmUpCard() { upgrade = Upgrade.B }, destination = CardDestination.Discard },
			],
			_ =>
			[
				new AStatus { targetPlayer = false, status = ModEntry.Instance.KokoroApi.DriveStatus.Underdrive, statusAmount = 1 },
				new AAddCard() { amount = 1, destination = CardDestination.Discard, card = new SetEmUpCard() }
			]
		};
}
			
