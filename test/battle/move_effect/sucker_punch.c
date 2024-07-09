#include "global.h"
#include "test/battle.h"

#ifdef ROGUE_DRAYANO
SINGLE_BATTLE_TEST("Sucker Punch does not switch mons")
{
    GIVEN {
        PLAYER(SPECIES_WOBBUFFET) {Ability(ABILITY_TORRENT); };
        OPPONENT(SPECIES_WOBBUFFET) {Ability(ABILITY_INSOMNIA); };
        OPPONENT(SPECIES_PONYTA) {Ability(ABILITY_INSOMNIA); };
    } WHEN {
        TURN { MOVE(opponent, MOVE_SUCKER_PUNCH); MOVE(player, MOVE_TACKLE); }
        TURN { }
    } SCENE {
        ANIMATION(ANIM_TYPE_MOVE, MOVE_SUCKER_PUNCH, opponent);
    }
}
#endif