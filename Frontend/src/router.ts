import VueRouter from 'vue-router';
import Home from "@/components/Home.vue";
import ErrorPage404 from "@/components/ErrorPage404.vue"
import QuizStart from "@/components/Game/QuizStart.vue";
import Quiz from "@/components/Game/Quiz.vue";
import HighscoreList from "@/components/Game/Highscore/HighscoreList.vue";
import HighscoreDetail from "@/components/Game/Highscore/HighscoreDetail.vue";

const routes = [
    { path: '/', component: Home },
    { path: '/quiz', redirect: '/quiz/start'},
    // @ts-ignore: static method "beforeEnter" exists on class QuizStart
    { path: '/quiz/start', component: QuizStart, beforeEnter: QuizStart.beforeEnter },
    // @ts-ignore: static method "beforeEnter" exists on class Quiz
    { path: '/quiz/play', component: Quiz, beforeEnter: Quiz.beforeEnter},
    { path: '/highscores', component: HighscoreList },
    { path: '/highscores/:id', component: HighscoreDetail },
    { path: '*', component: ErrorPage404 }
]

export default new VueRouter({
    mode: 'history',
    base: '/',
    routes
});