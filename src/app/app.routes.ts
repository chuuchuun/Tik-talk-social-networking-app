import { Routes } from '@angular/router';
import { LoginPageComponent } from './Pages/login-page/login-page.component';
import { SearchPageComponent } from './Pages/search-page/search-page.component';
import { ProfilePageComponent } from './Pages/profile-page/profile-page.component';
import { LayoutComponent } from './common-ui/layout/layout.component';
import { canActivateAuth } from './auth/access.guard';
import { SettingsPageComponent } from './Pages/settings-page/settings-page.component';
import { ChatsPageComponent } from './Pages/chats-page/chats-page.component';

export const routes: Routes = [
    {path: '', component:LayoutComponent, children: [
        {path: '', redirectTo: 'profile/me', pathMatch: 'full'},
        {path: 'search', component: SearchPageComponent},
        {path: 'profile/:id', component:ProfilePageComponent},
        {path: 'subscribers', component:ProfilePageComponent},
        {path: 'settings', component: SettingsPageComponent},
        {path: 'chats', component: ChatsPageComponent}
    ]
    ,canActivate: [canActivateAuth]
},
    {path: 'login', component: LoginPageComponent}
];
