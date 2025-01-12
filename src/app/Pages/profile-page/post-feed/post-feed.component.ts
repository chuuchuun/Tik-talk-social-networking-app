import { Component, inject, Input, input } from '@angular/core';
import { PostEnterComponent } from "../../../common-ui/post-enter/post-enter.component";
import { PostComponent } from "../../../common-ui/post/post.component";
import { ProfileService } from '../../../data/services/profile.service';
import { Profile } from '../../../data/Interfaces/profile.interface';

@Component({
  selector: 'app-post-feed',
  imports: [PostEnterComponent, PostComponent],
  templateUrl: './post-feed.component.html',
  styleUrl: './post-feed.component.scss'
})
export class PostFeedComponent {
  @Input() profile!: Profile;
}
