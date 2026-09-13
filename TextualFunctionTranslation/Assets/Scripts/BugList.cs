using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//I WAS TOO LAZY TO MAKE THIS A TXT FILE SO I GUESS ITS A .CS LMAOOOO
public class BugList
{
    /*
    * Known Bugs:
    *   - Animation compounding shift: animations move the character and other animations don't reanchor meaning positions shift
    *       - let animations play out
    *       - random buffer animation between every 2 that in 1 frame mixes this
    *       - reimport several offender animations
    *
    *   - UI Scaling: UI is bad
    *       - Swap from pixel to screen percent
    *       - swap out image swith actual images, probably min/max scaling constraints (hell)
    *
    *   - Some fuckass hex dist nullpointer
    *       - I don't even know maybe a unit died and unclaimed and someone wasn't told????
    *
    *   - Gross blinking: Hex squares don't blink correctly, the textmesh font is also getting blinked with square...
    *
    * 
    * Shit Features:
    *   - Lense toggle button is a terrible idea, also need some visualization over squares for the review stage so you're not just clicky clicky
    *   - Need more depth in tutorial, need to explain initiative, taunt, range, money
    *   - Wire up randomized: true dialogue ot randomizer effect lol
    *
    *
    */
}
