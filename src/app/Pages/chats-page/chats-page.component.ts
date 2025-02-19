import { Component, inject, signal } from '@angular/core';
import { ProfileService } from '../../data/services/profile.service';
import { ChatCardComponentComponent } from "../../common-ui/chat-card-component/chat-card-component.component";
import { PostEnterComponent } from "../../common-ui/post-enter/post-enter.component";
import { AsyncPipe } from '@angular/common';
import { RouterLink } from '@angular/router';
import { MessageSpaceComponent } from "./message-space/message-space.component";
import { Chat } from '../../data/Interfaces/chat.interface';

@Component({
  selector: 'app-chats-page',
  imports: [RouterLink, ChatCardComponentComponent, PostEnterComponent, AsyncPipe, MessageSpaceComponent],
  templateUrl: './chats-page.component.html',
  styleUrl: './chats-page.component.scss'
})
export class ChatsPageComponent {
  selectedChat:Chat | null = null 
  profileService = inject(ProfileService)
  me = this.profileService.me
  chats = signal(this.profileService.getMyChats())
  ngOnInit(){
    this.profileService.getMyChats()
    console.log(this.chats())
  }

  selectChat(chat: Chat){
    this.selectedChat = chat
  }
}
