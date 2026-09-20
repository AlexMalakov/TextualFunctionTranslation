using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResultsRaycast : MonoBehaviour
{
 
    [SerializeField] private CombatLog log;
    [SerializeField] private CombatManager cm;
    [SerializeField] private Player player;
    [SerializeField] private Player enemy;

    [SerializeField] private TutorialStage tut;

    private bool tutorialLockOn;
    private bool tutorialActive;

    private bool inResultsPhase = false;

    private CombatEvent curFrame;

    private Coroutine unitMovie;

    public void runResults(bool active) {
        this.inResultsPhase = active;

        if(!this.inResultsPhase) {
            this.endMovie();
        }
    }

    public void checkUserInteraction() {
        if(this.tutorialLockOn) {
            if(this.unitMovie != null) {
                this.endMovie();
            }
            return;
        }

        if (this.inResultsPhase && Input.GetMouseButtonDown(0)) {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit)) {
                if (hit.collider.gameObject.GetComponentInParent<Unit>() != null) {

                    this.onUnitPressed(hit.collider.gameObject.GetComponentInParent<Unit>());
                    return;
                }
            }
            this.endMovie();
        } else if(this.inResultsPhase && Input.GetMouseButtonDown(1)) {
            this.endMovie();
        }
    }

    private void endMovie() {
        if(this.unitMovie != null) {
            hideEvent(this.curFrame);
            this.tut.onMovieEnded();
            StopCoroutine(this.unitMovie);
            this.unitMovie = null;
            this.cm.resetToPostCombat();
        }
    }

    private void onUnitPressed(Unit u) {
        if(this.tutorialActive && !this.tut.isMovieUnit(u)) {
            return;
        }
        List<CombatEvent> uEvents = this.log.getUnitsEvents(u);
        this.cm.hideAllUnits();
        this.unitMovie = StartCoroutine(this.playUnitMovie(u, uEvents));
    }

    private IEnumerator playUnitMovie(Unit u, List<CombatEvent> events) {
        int pointer = 0;
        while(true) {
            this.curFrame = events[pointer];
            this.showEvent(this.curFrame);
            yield return new WaitForSeconds(GlobalData.MovieFrameDuration);
            this.hideEvent(events[pointer]);
            pointer = (pointer + 1) % events.Count;
        }
    }

    private void showEvent(CombatEvent ev) {
        switch(ev.eventType) {
            case CombatEventType.Kill:
                this.showFight(ev.source, ev.colat, ev.sourcePos, ev.colatPos, "movie_attack", "death");
                break;
            case CombatEventType.Attack:
                this.showFight(ev.source, ev.colat, ev.sourcePos, ev.colatPos, "movie_attack", "idle_combat");
                break;
            case CombatEventType.Death:
                this.showFight(ev.source, ev.colat, ev.sourcePos, ev.colatPos, "death", "movie_attack");
                break;
            case CombatEventType.Survival:
                ev.source.gameObject.SetActive(true);
                ev.source.fakePlaceOnSquare(ev.sourcePos);
                ev.source.triggerAnimation("victory_jump");
                ev.source.rotateTo((this.cm.isPlayerUnit(ev.source) ? this.player : this.enemy).getUnitRotation(false));
                break;
        }
    }

    private void showFight(Unit u1, Unit u2, HexSquare s1, HexSquare s2, string ani1, string ani2) {
        u1.gameObject.SetActive(true);
        u1.fakePlaceOnSquare(s1);
        u1.triggerAnimation(ani1);

        u2.gameObject.SetActive(true);
        u2.fakePlaceOnSquare(s2);
        u2.triggerAnimation(ani2);

        u1.transform.LookAt(u2.transform);
        u2.transform.LookAt(u1.transform);
    }

    private void hideEvent(CombatEvent ev) {
        switch(ev.eventType) {
            case CombatEventType.Kill:
                this.hideFight(ev.source, ev.colat);
                break;
            case CombatEventType.Attack:
                this.hideFight(ev.source, ev.colat);
                break;
            case CombatEventType.Death:
                this.hideFight(ev.source, ev.colat);
                break;
            case CombatEventType.Survival:
                ev.source.gameObject.SetActive(false);
                ev.source.fakePlaceOnSquare(ev.source.getPosition());
                ev.source.triggerAnimation("idle");
                ev.source.rotateTo((this.cm.isPlayerUnit(ev.source) ? this.player : this.enemy).getUnitRotation(false));
                break;
        }
    }

    private void hideFight(Unit u1, Unit u2) {
        u1.gameObject.SetActive(false);
        u1.fakePlaceOnSquare(u1.getPosition());
        u1.triggerAnimation("idle");

        u2.gameObject.SetActive(false);
        u2.fakePlaceOnSquare(u2.getPosition());
        u2.triggerAnimation("idle");

        u1.rotateTo((this.cm.isPlayerUnit(u1) ? this.player : this.enemy).getUnitRotation(false));
        u2.rotateTo((this.cm.isPlayerUnit(u2) ? this.player : this.enemy).getUnitRotation(false));
    }

    public void tutorialStarted(bool started) {
        this.tutorialActive = started;
    }

    public void tutorialLock(bool lockOn) {
        this.tutorialLockOn = lockOn;
    }
}