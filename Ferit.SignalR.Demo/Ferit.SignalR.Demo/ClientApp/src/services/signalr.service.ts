import { Inject, Injectable } from "@angular/core";
import { HubConnection, HubConnectionBuilder, ISubscription } from "@aspnet/signalr";
import { BehaviorSubject, Subject } from "rxjs";
import { RoomDetails } from "src/models/RoomDetails";
import { RoomState } from "../models/RoomState";

@Injectable({
  providedIn: "root",
})
export class SignalRService {
  private hubConnection: HubConnection;
  private stream: ISubscription<RoomState>;

  public roomState: Subject<RoomState>;
  public rooms: Subject<Array<RoomDetails>>;
  public isHubConnected: BehaviorSubject<boolean>;
  public isGameRunning: BehaviorSubject<boolean>;
  public userIsInRoom: Subject<boolean>;

  constructor(@Inject("WEB_BASE_URL") private baseUrl: string) {
    this.hubConnection = this.buildConnection();
    this.roomState = new Subject<RoomState>();
    this.rooms = new Subject<Array<RoomDetails>>();
    this.isHubConnected = new BehaviorSubject<boolean>(false);
    this.isGameRunning = new BehaviorSubject<boolean>(false);

    this.connectToService();
  }

  private buildConnection() {
    return new HubConnectionBuilder()
      .withUrl(this.baseUrl + "hub")
      .build();
  }

  private connectToService() {
    this.hubConnection
      .start()
      .then((c) => {
        this.setCallbacks();
        this.isHubConnected.next(true);
      })
      .catch((err) => {
        console.error("signalr catch", err);
        this.isHubConnected.next(false);
      });

    this.hubConnection.onclose((e) => {
      this.isHubConnected.next(false);
      console.error("signalr onclose", e);
    });
  }

  public startStream(room: string) {
    this.isGameRunning.next(true);
    this.stream = this.hubConnection.stream<RoomState>("Streaming", room).subscribe({
      next: (item) => {
        this.roomState.next(item);
      },
      complete: () => {
        this.isGameRunning.next(false);
      },
      error: (err) => {
        this.isGameRunning.next(false);
      },
    });
  }

  public keyPressed(room: string, dir: number) {
    this.hubConnection.invoke("ChangeDirection", room, dir);
  }

  public joinGame(room: string, user: string, color: number) {
    return this.hubConnection.invoke("UserJoin", room, user, color);
  }

  public createRoom(name: string, maxPlayers: number) {
    return this.hubConnection.invoke("RoomCreate", name, maxPlayers);
  }

  public removeRooms() {
    this.hubConnection.invoke("RoomsRemove");
  }

  public checkRoomName(room: string) {
    return this.hubConnection.invoke("CheckRoomName", room);
  }

  public getHighScores() {
    this.hubConnection.invoke("HighScores");
  }

  public userLeft(room: string) {
    this.stream.dispose();
    this.hubConnection.invoke("UserLeft", room);
  }

  checkConnection(room: string) {
    return this.hubConnection.invoke("IsUserInRoom", room);
  }

  private setCallbacks() {
    this.hubConnection.on("RoomStatusResponse", (v) => {
      this.rooms.next(v);
    });

    this.hubConnection.on("IsUserInRoom", (c) => {
      this.userIsInRoom = c;
    });

    this.hubConnection.on("GameOver", (c) => {
      alert(c);
      this.isGameRunning.next(false);
      this.stream.dispose();
    });
  }
}
