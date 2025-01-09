import { Component, inject } from '@angular/core';
import { ProfileHeaderComponent } from "../../common-ui/profile-header/profile-header.component";
import { ProfileService } from '../../data/services/profile.service';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { switchMap } from 'rxjs';
import { toObservable } from '@angular/core/rxjs-interop';
import { AsyncPipe, NgFor, NgIf } from '@angular/common';
import { PostEnterComponent } from "../../common-ui/post-enter/post-enter.component";

@Component({
  selector: 'app-profile-page',
  imports: [ProfileHeaderComponent, AsyncPipe, RouterLink, NgIf, PostEnterComponent],
  templateUrl: './profile-page.component.html',
  styleUrl: './profile-page.component.scss'
})
export class ProfilePageComponent {
  profileService = inject(ProfileService)
  route = inject(ActivatedRoute)
  me$ = toObservable(this.profileService.me)
  profile$ = this.route.params
  .pipe(
    switchMap(({id}) => {
      if(id == 'me') return this.me$
      return this.profileService.getAccount(id)
      })
  )
  subscribers$ = this.profile$.pipe(
    switchMap((profile) =>
      this.profileService.getSubscribersShortList(profile!.id, 5)
    )
  );
  subscribersAmount = this.profile$.pipe(
    switchMap((profile) =>
      this.profileService.getSubscribersAmount(profile!.id)
    )
  );
}
