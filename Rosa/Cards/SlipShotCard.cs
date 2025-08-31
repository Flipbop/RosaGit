using Nanoray.PluginManager;
using Nickel;
using System.Collections.Generic;
using System.Reflection;

namespace Flipbop.Cleo;

internal sealed class SlipShotCard : Card, IRegisterable
{
	private static Spr _nSprite;
	private static Spr _bSprite;

	public static void Register(IPluginPackage<IModManifest> package, IModHelper helper)
	{
		_nSprite = helper.Content.Sprites
			.RegisterSprite(package.PackageRoot.GetRelativeFile("assets/Cards/SlipShot.png")).Sprite;
		_bSprite = helper.Content.Sprites
			.RegisterSprite(package.PackageRoot.GetRelativeFile("assets/Cards/SlipShotFlip.png")).Sprite;
		helper.Content.Cards.RegisterCard(MethodBase.GetCurrentMethod()!.DeclaringType!.Name, new()
		{
			CardType = MethodBase.GetCurrentMethod()!.DeclaringType!,
			Meta = new()
			{
				deck = ModEntry.Instance.CleoDeck.Deck,
				rarity = ModEntry.GetCardRarity(MethodBase.GetCurrentMethod()!.DeclaringType!),
				upgradesTo = [Upgrade.A, Upgrade.B]
			},
			Art = _nSprite,
			Name = ModEntry.Instance.AnyLocalizations.Bind(["card", "SlipShot", "name"]).Localize
		});
	}

	public override CardData GetData(State state)
		=> new()
		{
			artTint = "996699",
			cost = upgrade == Upgrade.B ? 0 : 1,
			exhaust = upgrade == Upgrade.B,
			art = upgrade switch {
				Upgrade.B => _bSprite,
				_ => _nSprite,
			}
		};

	public override List<CardAction> GetActions(State s, Combat c)
		=> upgrade switch
		{
			Upgrade.B => [
				new AAttack { damage = GetDmg(s, 3), moveEnemy = 2},
			],
			Upgrade.A => [
				new AAttack { damage = GetDmg(s, 3), moveEnemy = -2},
			],
			_ => [
				new AAttack { damage = GetDmg(s, 2), moveEnemy = -1},
			]
		};
}
