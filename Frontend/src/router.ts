import VueRouter, { NavigationGuardNext, Route } from 'vue-router';
import Home from "@/components/Home.vue";
import ErrorPage404 from "@/components/ErrorPage404.vue"
import QuizStart from "@/components/Game/QuizStart.vue";
import Quiz from "@/components/Game/Quiz.vue";
import HighscoreList from "@/components/Game/Highscore/HighscoreList.vue";
import HighscoreDetail from "@/components/Game/Highscore/HighscoreDetail.vue";
import QuestionList from '@/components/QuestionEdit/QuestionList.vue'
import QuestionDetail from '@/components/QuestionEdit/QuestionDetail.vue'
import CategoryList from '@/components/CategoryEdit/CategoryList.vue';
import Profile from '@/components/Profile/Profile.vue';
import Login from '@/components/Profile/Login.vue';

const routes = [
    { path: '/', component: Home },
    { path: '/quiz', redirect: '/quiz/start'},
    // @ts-ignore: static method "beforeEnter" exists on class QuizStart
    { path: '/quiz/start', component: QuizStart, beforeEnter: QuizStart.beforeEnter },
    // @ts-ignore: static method "beforeEnter" exists on class Quiz
    { path: '/quiz/play', component: Quiz, beforeEnter: Quiz.beforeEnter},
    { path: '/highscores', component: HighscoreList },
    { path: '/highscores/:id', component: HighscoreDetail },
    // @ts-ignore: static method "beforeEnter" exists on class
    { path: '/questions', component: QuestionList, beforeEnter: QuestionList.beforeEnter  },
    // @ts-ignore: static method "beforeEnter" exists on class
    { path: '/questions/:method/:id?', component: QuestionDetail, beforeEnter: QuestionDetail.beforeEnter },
    // @ts-ignore: static method "beforeEnter" exists on class
    { path: '/categories', component: CategoryList, beforeEnter: CategoryList.beforeEnter },
    // @ts-ignore: static method "beforeEnter" exists on class
    { path: '/categories/:method/:id?', component: CategoryList, beforeEnter: CategoryList.beforeEnter },
    { path: '/profile', component: Profile },
    { path: '/profile/:method', component: Login },
    { path: '*', component: ErrorPage404 }
]

export default new VueRouter({
    mode: 'history',
    base: '/',
    routes
});