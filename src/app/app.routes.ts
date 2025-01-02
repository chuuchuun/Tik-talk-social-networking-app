import { Routes } from '@angular/router';
import { LoginPageComponent } from './Pages/login-page/login-page.component';
import { SearchPageComponent } from './Pages/search-page/search-page.component';
import { ProfilePageComponent } from './Pages/profile-page/profile-page.component';
import { LayoutComponent } from './common-ui/layout/layout.component';

export const routes: Routes = [
    {path: '', component:LayoutComponent, children: [
        {path: '', component: SearchPageComponent},
        {path: 'profile', component:ProfilePageComponent}
    ]},
    {path: 'login', component: LoginPageComponent}
];