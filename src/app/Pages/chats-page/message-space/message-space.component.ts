import { Component, EventEmitter, inject, Input, Output, SimpleChanges } from '@angular/core';
import { Chat } from '../../../data/Interfaces/chat.interface';
import { MessageCardComponent } from "../message-card/message-card.component";
import { ProfileService } from '../../../data/services/profile.service';
import { SendMessageComponent } from "../../../common-ui/send-message/send-message.component";
import { Message } from '../../../data/Interfaces/message.interface';
import { toSignal } from '@angular/core/rxjs-interop';
import { BehaviorSubject, Observable } from 'rxjs';
import { AsyncPipe } from '@angular/common';
import { RouterLink } from '@angular/router';
import { Profile } from '../../../data/Interfaces/profile.interface';


@Component({
  selector: 'app-message-space',
  imports: [MessageCardComponent, SendMessageComponent, AsyncPipe, RouterLink],
  templateUrl: './message-space.component.html',
  styleUrl: './message-space.component.scss'
})
export class MessageSpaceComponent {
    @Output() messageSentSpace = new EventEmitter<void>();
    @Input() chat!: Chat | null;
    profileService = inject(ProfileService);
    private messagesSubject = new BehaviorSubject<Message[]>([]);
    messages$ = this.messagesSubject.asObservable(); // Expose as observable for template binding
    me = this.profileService.me;
    anotherUser: number | null = null;
    picture: string | null = null; 
    lastMessage :string | null = null;
    firstName: string |null = null;
    lastName: string|null = null;
    isOptions: boolean = false;
    ngOnChanges(changes: SimpleChanges) {
      if (changes['chat'] && this.chat) {
        this.loadMessages(); // Reload messages when chat changes
        this.chat.userFirst;  // Access userFirst when chat is updated
        if(this.chat.userFirst == this.me()?.id){
          this.anotherUser = this.chat.userSecond;
        }else{
          this.anotherUser = this.chat.userFirst;
        }
        this.profileService.getAccount(this.anotherUser)
        .subscribe(res => {
          this.picture = res.avatarUrl; 
          this.firstName = res.firstName;
          this.lastName = res.lastName;
        });
      }
    }
    loadMessages() {
      if (!this.chat) return;
      this.profileService.getMessages(this.chat.id).subscribe(messages => {
        this.messagesSubject.next(messages); // Update BehaviorSubject
      });
    }

    refreshChats() {
      this.loadMessages(); // Fetch new messages
      this.messageSentSpace.emit(); // Notify parent
      console.log("Messages refreshed");
    }

    UnselectChat(){
      this.chat = null;
      this.refreshChats();
    }

    ShowOptions(){
      this.isOptions = true;
    }
}
