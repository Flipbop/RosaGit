using Nanoray.PluginManager;
using Nickel;
using System.Collections.Generic;
using System.Reflection;
using daisyowl.text;
using Shockah.Kokoro;


namespace Flipbop.Cleo;

internal sealed class SeekerBarrageCard : Card, IRegisterable
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
				.RegisterSprite(package.PackageRoot.GetRelativeFile("assets/Cards/SeekerBarrage.png")).Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "SeekerBarrage", "name"]).Localize
		});
	}

	public override CardData GetData(State state)
		=> new()
		{
			artTint = "996699",
			cost = 3,
			exhaust = true,
			description =
				ModEntry.Instance.Localizations.Localize(["card", "SeekerBarrage", "description", upgrade.ToString()])
		};

	public override List<CardAction> GetActions(State s, Combat c)
		=> upgrade switch
		{
			Upgrade.A => [
				new ASeekerBarrageDiscard{Amount = 1},
			],
			Upgrade.B => [
				new ASeekerBarrageExhaust{Amount = 1},
			],
			_ => [
				new ASeekerBarrage{Amount = 1},
			]
		};
	
	private sealed class Hook : IKokoroApi.IV2.ICardRenderingApi.IHook
	{
		public Font? ReplaceTextCardFont(IKokoroApi.IV2.ICardRenderingApi.IHook.IReplaceTextCardFontArgs args)
		{
			if (args.Card is not SeekerBarrageCard)
				return null;
			return ModEntry.Instance.KokoroApi.Assets.PinchCompactFont;
		}
	}
}

