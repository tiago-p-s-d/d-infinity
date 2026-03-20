import { Routes } from '@angular/router';
import { Login } from './components/auth/login/login';
import { NewUser } from './components/auth/new-user/new-user';
import { Home } from './components/home/home';
import { authGuard } from './guards/auth-guard';
import { StartYourOwn } from './components/start-your-own/start-your-own';
import { Systems } from './components/gameplay/systems/systems';
import { Items } from './components/gameplay/items/items';
import { Maps } from './components/gameplay/maps/maps';
import { Skills } from './components/gameplay/skills/skills';
import { Spells } from './components/gameplay/spells/spells';
import { Races } from './components/gameplay/races/races';
import { CharacterSheet } from './components/gameplay/character-sheet/character-sheet';
import { Currency } from './components/gameplay/currency/currency';

export const routes: Routes = [
  { path: '', redirectTo: 'login', pathMatch: 'full' },
  { path: 'login', component:  Login},
  { path: 'register', component: NewUser },
  { path: 'home', component: Home, canActivate: [authGuard] },
  { path: 'start-own', component: StartYourOwn, canActivate: [authGuard]},
  { path: 'dm/systems', component: Systems, canActivate: [authGuard]},
  { path: 'dm/items', component: Items, canActivate: [authGuard]},
  { path: 'dm/maps', component: Maps, canActivate: [authGuard]},
  { path: 'dm/skills', component: Skills, canActivate: [authGuard]},
  { path: 'dm/spells', component: Spells, canActivate: [authGuard]},
  { path: 'dm/races', component: Races, canActivate: [authGuard]},
  { path: 'dm/character-sheets', component: CharacterSheet, canActivate: [authGuard]},
  { path: 'dm/currency', component: Currency, canActivate: [authGuard]},

  




];