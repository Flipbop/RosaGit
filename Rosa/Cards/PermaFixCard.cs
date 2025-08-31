using Nanoray.PluginManager;
using Nickel;
using System.Collections.Generic;
using System.Reflection;
using daisyowl.text;
using Shockah.Kokoro;

namespace Flipbop.Cleo;

internal sealed class PermaFixCard : Card, IRegisterable
{
	public static void Register(IPluginPackage<IModManifest> package, IModHelper helper)
	{
		ModEntry.Instance.KokoroApi.CardRendering.RegisterHook(new Hook());

		helper.Content.Cards.RegisterCard(MethodBase.GetCurrentMethod()!.DeclaringType!.Name, new()
		{
			CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				deck = ModEntry.Instance.CleoDeck.Deck,
				rarity = ModEntry.GetCardRarity(MethodBase.GetCurrentMethod()!.DeclaringType!),
				upgradesTo = [Upgrade.A, Upgrade.B]
			},
			Art = helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("assets/Cards/PermaFix.png")).Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "PermaFix", "name"]).Localize
		});
	}

	public override CardData GetData(State state)
		=> new()
		{
			artTint = "996699",
			cost = upgrade switch
			{
				Upgrade.B => 3,
				Upgrade.A => 1,
				_ => 2
			},
			singleUse = true,
			retain = upgrade == Upgrade.A,
			description =
				ModEntry.Instance.Localizations.Localize([
					"card", "PermaFix", "description", upgrade.ToString()
				]),
		};

	public override List<CardAction> GetActions(State s, Combat c)
		=> upgrade switch
		{
			Upgrade.B => [
				new APermaFixB {Amount = 2},
			],
			_ => [
				new APermaFix {Amount = 1},
			],
		};
	
	private sealed class Hook : IKokoroApi.IV2.ICardRenderingApi.IHook
	{
		public Font? ReplaceTextCardFont(IKokoroApi.IV2.ICardRenderingApi.IHook.IReplaceTextCardFontArgs args)
		{
			if (args.Card is not PermaFixCard || args.Card.upgrade != Upgrade.B)
				return null;
			return ModEntry.Instance.KokoroApi.Assets.PinchCompactFont;
		}
	}
}