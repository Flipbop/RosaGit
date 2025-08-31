using Nanoray.PluginManager;


using Nickel;
using System.Collections.Generic;
using System.Reflection;
using daisyowl.text;
using Shockah.Kokoro;

namespace Flipbop.Cleo;

internal sealed class CleanSlateCard : Card, IRegisterable
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
			Art = ModEntry.Instance.CleanSlateSprite.Sprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "CleanSlate", "name"]).Localize
		});
	}

	public override CardData GetData(State state)
		=> new()
		{
			artTint = "996699",
			cost = upgrade == Upgrade.B ? 3 : 2,
			exhaust = upgrade != Upgrade.B,
			retain = upgrade == Upgrade.A,
			description = ModEntry.Instance.Localizations.Localize(["card", "CleanSlate", "description", upgrade.ToString()]),
		};

	public override List<CardAction> GetActions(State s, Combat c)
		=>
		[
			new ACleanSlate(),
			new ADiscountHand {Amount = -1}
		];
	private sealed class Hook : IKokoroApi.IV2.ICardRenderingApi.IHook
	{
		public Font? ReplaceTextCardFont(IKokoroApi.IV2.ICardRenderingApi.IHook.IReplaceTextCardFontArgs args)
		{
			if (args.Card is not CleanSlateCard)
				return null;
			return ModEntry.Instance.KokoroApi.Assets.PinchCompactFont;
		}
	}
}

