import { Component, OnInit, ViewChild } from "@angular/core";
import { FormBuilder, FormGroup, Validators } from "@angular/forms";
import { MatStepper } from "@angular/material";
import { Router } from "@angular/router";
import { SnakeColors } from "src/models/SnakeColors";
import { roomStorageName } from "src/models/constants";
import { RoomDetails } from "src/models/RoomDetails";
import { SignalRService } from "src/services/signalr.service";

@Component({
  selector: "app-home",
  templateUrl: "./home.component.html",
  styleUrls: ["./home.component.css"],
})
export class HomeComponent implements OnInit {
  @ViewChild("stepper", { static: true }) stepper: MatStepper;
  displayedColumns: string[] = ["room", "players", "actionButtons"];
  scoreColumns: string[] = ["player", "score"];

  public userForm: FormGroup;
  public roomForm: FormGroup;

  public roomsDetails: Array<RoomDetails> = [];
  public usedColors: Array<number>;
  public isCreateRoomClicked = false;
  skipRoomCreation: boolean;

  public colorEnum = SnakeColors;
  public colorsKeys = Object.keys(this.colorEnum).filter((f) => !isNaN(Number(f)));

  constructor(
    private signalRService: SignalRService,
    private formBuilder: FormBuilder,
    private router: Router
  ) {
    this.signalRService.rooms.subscribe((rooms) => {
      this.roomsDetails = rooms;
    });
  }

  ngOnInit() {
    this.userForm = this.formBuilder.group({
      userName: ["", [Validators.required, Validators.pattern(/[\S]/)]],
      color: [, Validators.required],
    });

    this.roomForm = this.formBuilder.group({
      room: ["", [Validators.required, Validators.pattern(/[\S]/)]],
      maxPlayers: [, Validators.required],
    });
  }

  ngAfterViewInit() {
    this.signalRService.removeRooms();
  }

  isRoomFull(roomDetails: RoomDetails): boolean {
    return roomDetails.playersCount >= roomDetails.maxPlayers;
  }

  toggleRoomCreation() {
    this.isCreateRoomClicked = !this.isCreateRoomClicked;
  }

  joinRoom(room: string, maxPlayers: number) {
    this.roomForm.setValue({ "room": room, "maxPlayers": maxPlayers });

    this.skipRoomCreation = true;
    this.isCreateRoomClicked = true;
    sessionStorage.setItem(roomStorageName, room);

    this.setColors();
    this.stepper.next();
  }

  onJoinGame() {
    let room = this.roomForm.value.room;
    let maxPlayers = Number.parseInt(this.roomForm.value.maxPlayers);

    if (!this.skipRoomCreation) {
      this.signalRService
        .createRoom(room, maxPlayers)
        .then(() => {
          sessionStorage.setItem(roomStorageName, room);
          this.signalRService.startStream(room);
          this.skipRoomCreation = false;

          this.connectToGame();
        })
        .catch((e) => {
          alert(e);
        });
    }
    else {
      this.connectToGame();
    }
  }

  connectToGame() {
    let room = this.roomForm.value.room;
    let userName = this.userForm.value.userName;
    let color = Number.parseInt(this.userForm.value.color);

    this.signalRService
      .joinGame(room, userName, color)
      .then(() => {
        this.signalRService.startStream(room);
        this.router.navigate(["game"]);
      })
      .catch((e) => {
        alert(e);
      });
  }

  onCreateRoom() {
    this.signalRService
      .checkRoomName(this.roomForm.value.room)
      .then((r) => {
        this.skipRoomCreation = false;
        this.setColors();
        this.stepper.next();
      })
      .catch((e) => {
        alert(e);
      });
  }

  isColorUsed(color: string): boolean {
    if (this.usedColors == undefined) {
      return false;
    }
    return this.usedColors.includes(Number.parseInt(color));
  }

  setColors() {
    let room = this.roomForm.value.room;

    this.roomsDetails.forEach((element) => {
      if (element.name == room) {
        this.usedColors = element.colors;
        return;
      }
    });
  }

  onBackButtonClick() {
    this.signalRService.removeRooms();
    this.stepper.previous();
    this.skipRoomCreation = false;
    this.usedColors = [];
  }
}
