import { Component, inject, Input, signal, SimpleChanges } from '@angular/core';
import { ProfileService } from '../../data/services/profile.service';
import { Chat } from '../../data/Interfaces/chat.interface';
import { last, map } from 'rxjs';

@Component({
  selector: 'app-chat-card-component',
  templateUrl: './chat-card-component.component.html',
  styleUrls: ['./chat-card-component.component.scss']
})
export class ChatCardComponentComponent {
  @Input() chat!: Chat; 

  profileService = inject(ProfileService);
  me = this.profileService.me;
  anotherUser: number | null = null;
  picture: string | null = null; 
  lastMessage :string | null = null;
  anotherUsername: string |null = null;

  ngOnChanges(changes: SimpleChanges) {
    if (changes['chat'] && this.chat) {
      this.lastMessage = this.chat.messages[this.chat.messages.length - 1];
      this.chat.userFirst;  // Access userFirst when chat is updated
      if(this.chat.userFirst == this.me()?.id){
        this.anotherUser = this.chat.userSecond;
      }else{
        this.anotherUser = this.chat.userFirst;
      }
      this.profileService.getAccount(this.anotherUser)
      .subscribe(res => {
        this.picture = res.avatarUrl; 
        this.anotherUsername = res.username;
      });
    }
  }
}
