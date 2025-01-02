import { Component, inject } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { ProfileCardComponent } from "./common-ui/profile-card/profile-card.component";
import { ProfileService } from './data/services/profile.service';
import { JsonPipe } from '@angular/common';
import { Profile } from './data/Interfaces/profile.interface';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, ProfileCardComponent, JsonPipe],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss'
})
export class AppComponent {
  title = 'tik-talk';
  
}
