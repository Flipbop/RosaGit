using System;
using FSPRO;
using Nickel;
using System.Collections.Generic;


namespace Flipbop.Cleo;

public sealed class AApologize : DynamicWidthCardAction
{
	public required int dmgRamp;
	public required bool peirce;

	public override void Begin(G g, State s, Combat c)
	{
		base.Begin(g, s, c);
		int index = c.hand.Count -1;
		int dmg = 1;
		int upgradeCounter = 0;
		while (index >= 0)
		{
			if (c.hand[index].upgrade != Upgrade.None)
			{
				upgradeCounter++;
			}
			index--;
		}

		for (int i = 0; i < upgradeCounter; i++)
		{
			c.Queue(new AAttack { damage = Card.GetActualDamage(s, dmg), piercing = peirce});
			dmg += dmgRamp;
		}
	}

	public override Icon? GetIcon(State s)
		=> new(ModEntry.Instance.ImpairHandIcon.Sprite, null, Colors.textMain);
	
	public override List<Tooltip> GetTooltips(State s)
      {
        List<Tooltip> tooltips = new List<Tooltip>();
        int x = s.ship.x;
        foreach (Part part in s.ship.parts)
        {
          if (part.type == PType.cannon && part.active)
          {
            if (s.route is Combat route && route.stuff.ContainsKey(x))
              route.stuff[x].hilight = 2;
            part.hilight = true;
          }
          ++x;
        }
        if (s.route is Combat route1)
        {
          foreach (StuffBase stuffBase in route1.stuff.Values)
          {
            if (stuffBase is JupiterDrone)
              stuffBase.hilight = 2;
          }
        }
        if (peirce)
          tooltips.Add(new TTGlossary("action.attackPiercing"));
        else
          tooltips.Add(new TTGlossary("action.attack.name"));
        return tooltips;
      }
}
