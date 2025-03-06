import { Component, EventEmitter, inject, Input, OnChanges, Output, SimpleChanges } from '@angular/core';
import { PostEnterComponent } from "../../../common-ui/post-enter/post-enter.component";
import { PostComponent } from "../../../common-ui/post/post.component";
import { ProfileService } from '../../../data/services/profile.service';
import { Profile } from '../../../data/Interfaces/profile.interface';
import { AsyncPipe } from '@angular/common';
import { BehaviorSubject, Observable } from 'rxjs';
import { Post } from '../../../data/Interfaces/post.interface';

@Component({
  selector: 'app-post-feed',
  imports: [PostEnterComponent, PostComponent, AsyncPipe],
  templateUrl: './post-feed.component.html',
  styleUrl: './post-feed.component.scss'
})
export class PostFeedComponent implements OnChanges {
  @Output() postCreatedParent = new EventEmitter<void>();
  @Input() profile!: Profile;
  profileService = inject(ProfileService);
  private postsSubject = new BehaviorSubject<Post[]>([]);
  posts$ = this.postsSubject.asObservable();

  ngOnChanges(changes: SimpleChanges) {
    if (changes['profile']?.currentValue) {
      this.loadPosts();
    }
  }

  loadPosts(){
    if(!this.profile) return;
    this.profileService.getPosts(this.profile.id).subscribe(posts => {
      this.postsSubject.next(posts);
    })

  }
  refreshPosts(){
    console.log("Got notified child")
    this.loadPosts();
    this.postCreatedParent.emit();
  }
}
