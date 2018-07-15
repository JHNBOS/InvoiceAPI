import { Component, OnInit } from '@angular/core';
import { DomSanitizer } from '@angular/platform-browser';
import { ActivatedRoute, NavigationEnd, Router } from '@angular/router';
import { filter, map, mergeMap } from 'rxjs/operators';
import { AuthenticationService } from './shared/authentication.service';
import User from './shared/models/user.model';

@Component({
    selector: 'app-root',
    templateUrl: './app.component.html',
    styleUrls: ['./app.component.scss']
})
export class AppComponent implements OnInit {
    currentUser: User = null;
    isSignedIn = false;
    title: string;

    constructor(private authService: AuthenticationService, private route: ActivatedRoute,
        private router: Router, private _sanitizer: DomSanitizer) { }

    ngOnInit() {
        this.checkIfLoggedIn();
        this.getCurrentRouteTitle();
    }

    checkIfLoggedIn() {
        this.currentUser = JSON.parse(sessionStorage.getItem('signedInUser'));
        if (this.currentUser != null) {
            this.isSignedIn = true;
        } else {
            this.isSignedIn = false;
        }
    }

    getCurrentRouteTitle() {
        this.router.events
            .pipe(filter(event => event instanceof NavigationEnd))
            .pipe(map(() => this.route))
            .pipe(map(route => {
                while (route.firstChild) { route = route.firstChild; }
                return route;
            }))
            .pipe(filter(route => route.outlet === 'primary'))
            .pipe(mergeMap(route => route.data))
            .subscribe((event) => this.title = event['title']);
    }

    signOut() {
        this.authService.logout();
        this.checkIfLoggedIn();

        if (!this.isSignedIn) {
            this.router.navigate(['/login']);
        }
    }
}
