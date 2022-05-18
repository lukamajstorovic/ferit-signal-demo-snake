import { Component, ElementRef, OnDestroy, ViewChild } from "@angular/core";
import { MatTableDataSource } from "@angular/material";
import { Router } from "@angular/router";
import { fromEvent, Subscription } from "rxjs";
import { SnakeColors } from "src/models/SnakeColors";
import { roomStorageName } from "src/models/constants";
import { PlayerState } from "src/models/PlayerState";
import { SignalRService } from "src/services/signalr.service";
import { DrawHelper } from "../helpers/drawHelper";

@Component({
  selector: "app-game",
  templateUrl: "./game.component.html",
  styleUrls: ["./game.component.css"],
})
export class GameComponent implements OnDestroy {
  @ViewChild("canvas", { static: true })
  canvas: ElementRef<HTMLCanvasElement>;

  private eventListener: Subscription;

  public room: string;
  public datasource: MatTableDataSource<PlayerState>;
  public displayedColumns: string[] = ["name", "score"];
  public colorsEnum = SnakeColors;

  constructor(
    private drawHelper: DrawHelper,
    private signalRService: SignalRService,
    //private keyHelper: KeyHelper,
    private router: Router
  ) { }

  ngAfterViewInit() {
    this.drawHelper.initialize(this.canvas);

    this.signalRService.roomState.subscribe((v) => {
      this.drawHelper.render(v);
      this.datasource = new MatTableDataSource(v.playerStates);
    });

    this.signalRService.isGameRunning.subscribe((running) => {
      if (!running) {
        this.navigateToHome();
      }
    });

    /*
    this.eventListener = fromEvent(window, "keydown")
      .subscribe((event: KeyboardEvent) => {
        this.keyHelper.keyPress(event);
      });
      */
    this.room = sessionStorage.getItem(roomStorageName);
  }

  ngOnDestroy() {
    this.eventListener.unsubscribe();
    this.signalRService.userLeft(this.room);
    sessionStorage.setItem(roomStorageName, null);
  }

  navigateToHome() {
    this.router.navigate([""]);
  }

  onResize() {
    this.drawHelper.initialize(this.canvas);
  }
}
