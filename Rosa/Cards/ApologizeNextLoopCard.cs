using Nanoray.PluginManager;
using Nickel;
using System.Collections.Generic;
using System.Reflection;
using daisyowl.text;
using Shockah.Kokoro;

namespace Flipbop.Cleo;

internal sealed class ApologizeNextLoopCard : Card, IRegisterable
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
			Art = helper.Content.Sprites
				.RegisterSprite(package.PackageRoot.GetRelativeFile("assets/Cards/ApologizeNextLoop.png")).Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "ApologizeNextLoop", "name"]).Localize
		});
	}

	public override CardData GetData(State state)
		=> new()
		{
			artTint = "996699",
			cost = 3,
			description =
				ModEntry.Instance.Localizations.Localize([
					"card", "ApologizeNextLoop", "description", upgrade.ToString()
				]),
			exhaust = true,
		};

	public override List<CardAction> GetActions(State s, Combat c)
		=> upgrade switch
		{
			Upgrade.A =>
			[
				new AApologize {dmgRamp = 2, peirce = false},
			],
			Upgrade.B => [
				new AApologize {dmgRamp = 1, peirce = true},
			],
			_ => [
				new AApologize {dmgRamp = 1, peirce = false},
			]
		};
	private sealed class Hook : IKokoroApi.IV2.ICardRenderingApi.IHook
	{
		public Font? ReplaceTextCardFont(IKokoroApi.IV2.ICardRenderingApi.IHook.IReplaceTextCardFontArgs args)
		{
			if (args.Card is not ApologizeNextLoopCard)
				return null;
			return ModEntry.Instance.KokoroApi.Assets.PinchCompactFont;
		}
	}
}
	

