using FSPRO;
using Nanoray.PluginManager;
using Nickel;
using System.Collections.Generic;
using System.Reflection;

namespace Flipbop.Cleo;

internal sealed class ResourceSwapCard : Card, IRegisterable
{
	private static Spr _flippedSprite;
	private static Spr _floppedSprite;
	private static Spr _aSprite;
	private static Spr _bSprite;

	public static void Register(IPluginPackage<IModManifest> package, IModHelper helper)
	{
		_flippedSprite = helper.Content.Sprites
			.RegisterSprite(package.PackageRoot.GetRelativeFile("assets/Cards/ResourceSwapTop.png")).Sprite;
		_floppedSprite = helper.Content.Sprites
			.RegisterSprite(package.PackageRoot.GetRelativeFile("assets/Cards/ResourceSwapBottom.png")).Sprite;
		_aSprite = helper.Content.Sprites
			.RegisterSprite(package.PackageRoot.GetRelativeFile("assets/Cards/ResourceSwapA.png")).Sprite;
		_bSprite = helper.Content.Sprites
			.RegisterSprite(package.PackageRoot.GetRelativeFile("assets/Cards/ResourceSwapB.png")).Sprite;
		helper.Content.Cards.RegisterCard(MethodBase.GetCurrentMethod()!.DeclaringType!.Name, new()
		{
			CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				deck = ModEntry.Instance.CleoDeck.Deck,
				rarity = ModEntry.GetCardRarity(MethodBase.GetCurrentMethod()!.DeclaringType!),
				upgradesTo = [Upgrade.A, Upgrade.B]
			},
			Art = _flippedSprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "ResourceSwap", "name"]).Localize
		});
		
	}

	public override CardData GetData(State state)
		=> new()
		{
			artTint = "FFFFFF",
			cost = 1,
			floppable = upgrade == Upgrade.None,
			art = upgrade switch {
				Upgrade.A => _aSprite,
				Upgrade.B => _bSprite,
				_ => flipped ? _floppedSprite : _flippedSprite,
			}
			
		};

	public override List<CardAction> GetActions(State s, Combat c)
		=> upgrade switch
		{
			Upgrade.A => [
				new AStatus { targetPlayer = true, status = Status.shield, statusAmount = 2 },
				new AStatus { targetPlayer = true, status = Status.tempShield, statusAmount = 2 },
				new AImpairSelf{id = this.uuid},
			],
			Upgrade.B => [
				new AStatus { targetPlayer = true, status = Status.evade, statusAmount = 2 },
				new AImpairSelf{id = this.uuid},
			],
			_ => [
				new AAttack {disabled = flipped, damage = GetDmg(s, 2) },
				new AImproveASelf {disabled = flipped, id = this.uuid},
				new ADummyAction(),
				new AAttack {disabled = !flipped, damage = GetDmg(s, 2) },
				new AImproveBSelf {disabled = !flipped, id = this.uuid},
			]
		};
}