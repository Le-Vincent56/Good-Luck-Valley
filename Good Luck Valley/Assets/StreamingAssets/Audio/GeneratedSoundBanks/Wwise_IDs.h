/////////////////////////////////////////////////////////////////////////////////////////////////////
//
// Audiokinetic Wwise generated include file. Do not edit.
//
/////////////////////////////////////////////////////////////////////////////////////////////////////

#ifndef __WWISE_IDS_H__
#define __WWISE_IDS_H__

#include <AK/SoundEngine/Common/AkTypes.h>

namespace AK
{
    namespace EVENTS
    {
        static const AkUniqueID PAUSE_MUSIC = 2735935537U;
        static const AkUniqueID PLAY_AMBIENCE2D_MAIN = 2609132508U;
        static const AkUniqueID PLAY_AMBIENCEBED_MAIN = 1808419977U;
        static const AkUniqueID PLAY_MUSHROOM_BOUNCE = 4286947103U;
        static const AkUniqueID PLAY_MUSHROOM_GROW = 1531298894U;
        static const AkUniqueID PLAY_MUSHROOM_PICKUP = 195236087U;
        static const AkUniqueID PLAY_MUSIC = 2932040671U;
        static const AkUniqueID PLAY_PLAYER_FALL = 1306343379U;
        static const AkUniqueID PLAY_PLAYER_IMPACTS = 2773402369U;
        static const AkUniqueID PLAY_PLAYER_JUMP = 562256996U;
        static const AkUniqueID PLAY_PLAYER_LAND = 4249207015U;
        static const AkUniqueID PLAY_PLAYER_SLIDE = 2425231895U;
        static const AkUniqueID PLAY_PLAYER_SLIDE_END = 3208332793U;
        static const AkUniqueID PLAY_TESTBEEP = 1955951874U;
        static const AkUniqueID PLAY_TESTBEEP_3D = 1681067324U;
        static const AkUniqueID PLAY_TESTBEEP_LOOP = 523232931U;
        static const AkUniqueID PLAY_TESTBEEP_LOOP_3D = 3475882615U;
        static const AkUniqueID RESUME_MUSIC = 2940177080U;
        static const AkUniqueID STOP_MUSHROOM_PICKUP = 3275097373U;
        static const AkUniqueID STOP_MUSIC = 2837384057U;
        static const AkUniqueID STOP_PLAYER_FALL = 3680109729U;
        static const AkUniqueID STOP_PLAYER_IMPACTS = 2961941791U;
        static const AkUniqueID STOP_PLAYER_SLIDE = 3309779001U;
        static const AkUniqueID STOP_TESTBEEP_LOOP = 2989390457U;
        static const AkUniqueID STOP_TESTBEEP_LOOP_3D = 304008745U;
    } // namespace EVENTS

    namespace STATES
    {
        namespace STATES_AREA
        {
            static const AkUniqueID GROUP = 1161428141U;

            namespace STATE
            {
                static const AkUniqueID NONE = 748895195U;
                static const AkUniqueID STATE_CAVE = 3843590704U;
                static const AkUniqueID STATE_FOREST = 4216640728U;
            } // namespace STATE
        } // namespace STATES_AREA

        namespace STATES_FOREST_P1_CUES
        {
            static const AkUniqueID GROUP = 1606666624U;

            namespace STATE
            {
                static const AkUniqueID NONE = 748895195U;
                static const AkUniqueID STATE_FOREST_P1_END = 506127700U;
                static const AkUniqueID STATE_FOREST_P1_FADE = 2025880267U;
                static const AkUniqueID STATE_FOREST_P1_INTRO = 3438870713U;
                static const AkUniqueID STATE_FOREST_P1_ONE = 4262636943U;
                static const AkUniqueID STATE_FOREST_P1_THREE = 665932511U;
                static const AkUniqueID STATE_FOREST_P1_TWO = 123052653U;
            } // namespace STATE
        } // namespace STATES_FOREST_P1_CUES

        namespace STATES_GAME
        {
            static const AkUniqueID GROUP = 708499520U;

            namespace STATE
            {
                static const AkUniqueID IN_GAME = 2967546505U;
                static const AkUniqueID IN_MENU = 1631528850U;
                static const AkUniqueID NONE = 748895195U;
            } // namespace STATE
        } // namespace STATES_GAME

        namespace STATES_PHASES
        {
            static const AkUniqueID GROUP = 1702361322U;

            namespace STATE
            {
                static const AkUniqueID NONE = 748895195U;
                static const AkUniqueID STATE_INTRO = 1240403123U;
                static const AkUniqueID STATE_PHASEONE = 3548780380U;
                static const AkUniqueID STATE_PHASETWO = 3930281454U;
            } // namespace STATE
        } // namespace STATES_PHASES

        namespace STATES_PLAYER
        {
            static const AkUniqueID GROUP = 1857190249U;

            namespace STATE
            {
                static const AkUniqueID NONE = 748895195U;
            } // namespace STATE
        } // namespace STATES_PLAYER

        namespace STATES_ROOM
        {
            static const AkUniqueID GROUP = 1500888219U;

            namespace STATE
            {
                static const AkUniqueID NONE = 748895195U;
                static const AkUniqueID STATE_INDOOR = 3825359022U;
                static const AkUniqueID STATE_OUTDOOR = 3990655221U;
            } // namespace STATE
        } // namespace STATES_ROOM

    } // namespace STATES

    namespace SWITCHES
    {
        namespace AMBIENCEBIOMESWITCH
        {
            static const AkUniqueID GROUP = 1097908491U;

            namespace SWITCH
            {
                static const AkUniqueID CAVE = 4122393694U;
                static const AkUniqueID FOREST = 491961918U;
            } // namespace SWITCH
        } // namespace AMBIENCEBIOMESWITCH

        namespace DISTANCESWITCH
        {
            static const AkUniqueID GROUP = 3582311692U;

            namespace SWITCH
            {
                static const AkUniqueID FIFTEEN = 787714852U;
                static const AkUniqueID FIFTY = 614582933U;
                static const AkUniqueID FIVE = 2611770117U;
                static const AkUniqueID ONE = 1064933119U;
                static const AkUniqueID ONEHUNDRED = 1461002489U;
                static const AkUniqueID TEN = 913095698U;
                static const AkUniqueID TWENTYFIVE = 3772991054U;
                static const AkUniqueID TWOFIVE = 2688829901U;
            } // namespace SWITCH
        } // namespace DISTANCESWITCH

        namespace GROUNDMATSWITCH
        {
            static const AkUniqueID GROUP = 3260060478U;

            namespace SWITCH
            {
                static const AkUniqueID DIRT = 2195636714U;
                static const AkUniqueID GRASS = 4248645337U;
                static const AkUniqueID STONE = 1216965916U;
            } // namespace SWITCH
        } // namespace GROUNDMATSWITCH

        namespace MUSHROOMCHAINSWITCH
        {
            static const AkUniqueID GROUP = 1980707854U;

            namespace SWITCH
            {
                static const AkUniqueID MAX = 1048449613U;
                static const AkUniqueID ONE = 1064933119U;
                static const AkUniqueID THREE = 912956111U;
                static const AkUniqueID TWO = 678209053U;
            } // namespace SWITCH
        } // namespace MUSHROOMCHAINSWITCH

        namespace MUSHROOMGROUNDMATSWITCH
        {
            static const AkUniqueID GROUP = 1426187460U;

            namespace SWITCH
            {
                static const AkUniqueID DIRT = 2195636714U;
                static const AkUniqueID GRASS = 4248645337U;
                static const AkUniqueID STONE = 1216965916U;
            } // namespace SWITCH
        } // namespace MUSHROOMGROUNDMATSWITCH

        namespace PLAYERFALLSPEEDSWITCH
        {
            static const AkUniqueID GROUP = 2054088970U;

            namespace SWITCH
            {
                static const AkUniqueID FASTFALL = 3857355556U;
                static const AkUniqueID MEDIUMFALL = 3946497219U;
                static const AkUniqueID SLOWFALL = 3936714913U;
            } // namespace SWITCH
        } // namespace PLAYERFALLSPEEDSWITCH

        namespace PLAYERSPEEDSWITCH
        {
            static const AkUniqueID GROUP = 2051106367U;

            namespace SWITCH
            {
                static const AkUniqueID CRAWL = 3115216662U;
                static const AkUniqueID RUN = 712161704U;
                static const AkUniqueID WALK = 2108779966U;
            } // namespace SWITCH
        } // namespace PLAYERSPEEDSWITCH

    } // namespace SWITCHES

    namespace GAME_PARAMETERS
    {
        static const AkUniqueID RTPC_DISTANCE = 262290038U;
        static const AkUniqueID RTPC_FIREFLYCOUNT = 4221177039U;
        static const AkUniqueID RTPC_MUSHROOMCHAIN = 1867215876U;
        static const AkUniqueID RTPC_PLAYERFALLSPEED = 2290256636U;
        static const AkUniqueID RTPC_PLAYERSPEED = 2653406601U;
    } // namespace GAME_PARAMETERS

    namespace BANKS
    {
        static const AkUniqueID INIT = 1355168291U;
        static const AkUniqueID GAME_SOUNDBANK = 3626155605U;
    } // namespace BANKS

    namespace BUSSES
    {
        static const AkUniqueID AMBIENCE_2D = 1705038622U;
        static const AkUniqueID AMBIENCE_3D = 1721816425U;
        static const AkUniqueID AMBIENCEBEDS_2D = 1890054062U;
        static const AkUniqueID MASTER_AUDIO_BUS = 3803692087U;
        static const AkUniqueID MASTER_AMBIENCE = 4286911030U;
        static const AkUniqueID MASTER_MUSHROOM = 2228101040U;
        static const AkUniqueID MASTER_MUSIC = 1900298039U;
        static const AkUniqueID MASTER_NPC = 3591553651U;
        static const AkUniqueID MASTER_PLAYER = 2453119373U;
        static const AkUniqueID MUSHROOM_BOUNCE = 4126136622U;
        static const AkUniqueID MUSHROOM_GROW = 4011678951U;
        static const AkUniqueID MUSHROOM_PICKUP = 1919967482U;
        static const AkUniqueID PLAYER_FALL = 2551268862U;
        static const AkUniqueID PLAYER_JUMP = 1305133589U;
        static const AkUniqueID PLAYER_LAND = 3629196698U;
        static const AkUniqueID PLAYER_LOCOMOTION = 1375983526U;
        static const AkUniqueID PLAYER_LOCOMOTION_IMPACTS = 2940948846U;
        static const AkUniqueID PLAYER_LOCOMOTION_SLIDE = 3614903352U;
    } // namespace BUSSES

    namespace AUX_BUSSES
    {
        static const AkUniqueID REVERB_CAVE = 323187407U;
        static const AkUniqueID REVERB_LARGEROOM = 591468168U;
        static const AkUniqueID REVERB_OUTDOOR = 1578973140U;
        static const AkUniqueID REVERB_SMALLROOM = 3292698384U;
        static const AkUniqueID REVERBS = 3545700988U;
    } // namespace AUX_BUSSES

    namespace AUDIO_DEVICES
    {
        static const AkUniqueID NO_OUTPUT = 2317455096U;
        static const AkUniqueID SYSTEM = 3859886410U;
    } // namespace AUDIO_DEVICES

}// namespace AK

#endif // __WWISE_IDS_H__
