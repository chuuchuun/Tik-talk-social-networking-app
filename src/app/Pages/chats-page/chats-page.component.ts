import { Component, inject, signal } from '@angular/core';
import { ProfileService } from '../../data/services/profile.service';
import { ChatCardComponentComponent } from "../../common-ui/chat-card-component/chat-card-component.component";
import { PostEnterComponent } from "../../common-ui/post-enter/post-enter.component";
import { AsyncPipe } from '@angular/common';

@Component({
  selector: 'app-chats-page',
  imports: [ChatCardComponentComponent, PostEnterComponent, AsyncPipe],
  templateUrl: './chats-page.component.html',
  styleUrl: './chats-page.component.scss'
})
export class ChatsPageComponent {
  profileService = inject(ProfileService)
  me = this.profileService.me
  chats = signal(this.profileService.getMyChats())
  ngOnInit(){
    this.profileService.getMyChats()
    console.log(this.chats())
  }
}
