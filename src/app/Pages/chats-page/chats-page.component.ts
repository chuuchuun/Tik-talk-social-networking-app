import { Component, inject } from '@angular/core';
import { ProfileService } from '../../data/services/profile.service';
import { ChatCardComponentComponent } from "../../common-ui/chat-card-component/chat-card-component.component";
import { PostEnterComponent } from "../../common-ui/post-enter/post-enter.component";

@Component({
  selector: 'app-chats-page',
  imports: [ChatCardComponentComponent, PostEnterComponent],
  templateUrl: './chats-page.component.html',
  styleUrl: './chats-page.component.scss'
})
export class ChatsPageComponent {
  profileService = inject(ProfileService)
  me = this.profileService.me


}
