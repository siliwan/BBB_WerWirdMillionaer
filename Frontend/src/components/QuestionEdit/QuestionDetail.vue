<template>
    <div class="questionDetail">
        <b-alert :show="hasWarning" variant="danger">{{warning}}</b-alert>
        <b-container>
            <b-overlay :show="isLoading">
                <b-form @submit="onSubmit" @reset="onReset" @change="onChange">
                    <b-form-row>
                        <p class="h2"><router-link to="/questions"><b-icon icon="arrow-left"></b-icon></router-link></p>
                    </b-form-row>
                    <b-form-row>
                        <b-col>
                            <b-form-group label="Id" label-for="model_id" v-if="method == 'edit'">
                                <b-form-input id="model_id" type="text" v-model="model.id" disabled />
                            </b-form-group>
                        </b-col>
                    </b-form-row>
                    <b-form-row>
                        <b-col cols="4">
                            <b-form-group label="Answers">
                                <b-col>
                                    <b-form-group label="Answer 1" label-for="model_answers_1">
                                        <b-form-radio v-model="correctAnswer" value="1">
                                            <b-form-input id="model_answers_1" ref="model_answers_1" v-model="firstAnswer.answerText" required/>
                                            <small v-if="correctAnswer == 1">Correct Answer</small>
                                        </b-form-radio>
                                    </b-form-group>
                                </b-col>
                                <b-col>
                                    <b-form-group label="Answer 2" label-for="model_answers_2">
                                        <b-form-radio v-model="correctAnswer" value="2">
                                            <b-form-input id="model_answers_2" ref="model_answers_2" v-model="secondAnswer.answerText" required/>
                                            <small v-if="correctAnswer == 2">Correct Answer</small>
                                        </b-form-radio>
                                    </b-form-group>
                                </b-col>
                                <b-col>
                                    <b-form-group label="Answer 3" label-for="model_answers_3">
                                        <b-form-radio v-model="correctAnswer" value="3">
                                            <b-form-input id="model_answers_3" ref="model_answers_3" v-model="thirdAnswer.answerText" required/>
                                            <small v-if="correctAnswer == 3">Correct Answer</small>
                                        </b-form-radio>
                                    </b-form-group>
                                </b-col>
                                <b-col>
                                    <b-form-group label="Answer 4" label-for="model_answers_4">
                                        <b-form-radio v-model="correctAnswer" value="4">
                                            <b-form-input id="model_answers_4" ref="model_answers_4" v-model="fourthAnswer.answerText" required/>
                                            <small v-if="correctAnswer == 4">Correct Answer</small>
                                        </b-form-radio>
                                    </b-form-group>
                                </b-col>
                            </b-form-group>
                        </b-col>
                        <b-col cols="8">
                            <b-form-group label="Question Text" label-for="model_questionText">
                                <b-form-textarea id="model_questionText" v-model="model.questionText" required/>
                            </b-form-group>
                        </b-col>
                        <b-col cols="12">
                            <b-form-group label="Category" label-for="model_category">
                                <b-form-select id="model_category" v-model="categorySelection" :options="categoryOptions" required/>
                            </b-form-group>
                        </b-col>
                    </b-form-row>
                    <b-form-row>
                        <b-button-group>
                            <b-button :variant="method === 'new' ? 'primary' : 'success'" type="submit" :disabled="this.method == 'edit' && !hasBeenEdited" v-text="saveNewButtonText" />
                            <b-button variant="primary" type="reset" :disabled="this.method == 'edit' && !hasBeenEdited">Cancel</b-button>
                        </b-button-group>
                    </b-form-row>
                </b-form>
            </b-overlay>
        </b-container>
    </div>
</template>

<script lang="ts">
    import QuestionCrud from "@/components/CRUD/QuestionCrud";
    import { Answer, Question, Category } from "@/ResponseTypes";
    import { Component, Vue } from 'vue-property-decorator';
    import CategoryCrud from "../CRUD/CategoryCrud";
    import AnswerCrud from "../CRUD/AnswerCrud";

    //@ts-ignore Import works just fine
    import { BFormInput } from "node_modules/bootstrap-vue/src";
import { NavigationGuardNext, Route } from "node_modules/vue-router/types/router";
import AuthApi from "@/services/AuthApi";

    type crudMethod = 'new' | 'edit' | 'invalid';

    @Component({
        components: {
            
        }
    })
    export default class QuestionDetail extends Vue {
        private isLoading: boolean = true;
        private model: Question = new Question();
        private categoriesAvailable: Category[] = [];
        private warning: string = '';
        private hasBeenEdited = false;
        private currentAnswerSelected = 1;

        constructor() {
            super();

        }

        async mounted() {
            if(this.method == 'invalid') {
                this.$router.go(-1);
            }

            if(this.method == 'edit') {
                if(typeof this.id != 'number' || isNaN(this.id)) {
                     this.$router.push("/questions");
                }

                this.reload();
            } else {
                this.categoriesAvailable = await CategoryCrud.getAll();
                this.ensureAnswersCreated();
                this.isLoading = false;
            }
        }

        async reload() {
            this.isLoading = true;
            this.hasBeenEdited = false;
            try {
                await this.reloadCategories();
                this.model = await QuestionCrud.get(this.id);
                this.model.answers.$values.forEach((element, index) => {
                    if(element.isCorrect) {
                        this.currentAnswerSelected = index+1;
                    }
                });
            } catch (error) {
                
            }
            this.isLoading = false;
        }

        async reloadCategories() {
            this.categoriesAvailable = await CategoryCrud.getAll();
            this.category = this.categoriesAvailable[0]
        }

        async onChange() {
            this.hasBeenEdited = true;
        }

        validate(): boolean {
            let isValid = true;

            if(this.model === undefined) isValid = false;
            if(this.model.questionText === undefined) isValid = false;
            if(this.model.questionText?.trim() === '') isValid = false;
            if(this.model.category === undefined) isValid = false;
            if(this.model.answers === undefined) isValid = false;
            if(this.model.answers.$values === undefined) isValid = false;
            
            for (let i = 0; i < this.model.answers?.$values?.length; i++) {
                const answer = this.model.answers.$values[i];
                if(answer === undefined) isValid = false;
                if(answer.answerText === undefined) isValid = false;
                if(answer.answerText?.trim() === '') isValid = false;
                if(answer.isCorrect === undefined) isValid = false;
            }

            if(this.model.answers?.$values?.filter(answer => answer.isCorrect).length !== 1) isValid = false;

            return isValid;
        }

        async onSubmit(evt: Event) {
            evt.preventDefault();
            this.answerFallback();
            if(!this.validate())
            {
                return;
            }

            if(this.method == 'new') {
                await this.createQuestion();
            } else if(this.method == 'edit') {
                await this.saveQuestion()
                this.reload();
            }
        }

        answerFallback() {
            for (let i = 0; i < this.model.answers?.$values?.length; i++) {
                const answer = this.model.answers.$values[i];
                if(answer.answerText === undefined || answer.answerText.trim() === '') {
                    let el = this.$refs[`model_answers_${i+1}`];

                    if(el == undefined) {
                        continue;
                    }

                    if(Array.isArray(el)) {
                        el = el[0];
                    }

                    answer.answerText = ((el as BFormInput).$el as HTMLInputElement).value;
                }
            }
        }

        async saveQuestion() {
            try {
                //Step 1: Save question
                await QuestionCrud.put(this.model.id, this.model.category.id, this.model.questionText)

                //Step 2: Save answers

                for (let i = 0; i < this.model.answers.$values.length; i++) {
                    const answer = this.model.answers.$values[i];
                    await AnswerCrud.update(answer.id, answer.answerText, answer.isCorrect);
                }

                this.$bvToast.toast(`Question with id ${this.id} has been saved!`, {
                    title: "Saved",
                    variant: "success",
                    solid: true,
                    toaster: 'b-toaster-top-right'
                });
            } catch (error) {
                console.error(error)
            }

            //Step 3: ???
            //Step 4: profit!
            
        }

        async createQuestion() {
            let answers = this.model.answers.$values.map(answer => {
                return {
                    answerText: answer.answerText,
                    isCorrect: answer.isCorrect
                }
            });
            await QuestionCrud.post(this.model.questionText, this.model.category.id, answers);
            this.$router.push("/questions")
        }


        async onReset() {
            await this.reload();
        }

        get correctAnswer() {
            return this.currentAnswerSelected;
        }

        set correctAnswer(value) {
            this.model.answers.$values[this.currentAnswerSelected - 1].isCorrect = false;
            this.model.answers.$values[value - 1].isCorrect = true;
            this.currentAnswerSelected = value;
            console.log(`${value - 1} set to true!`)
        }

        get categoryOptions() {
            return this.categoriesAvailable.map(c => {
                return { value: c.id, text: c.name };
            })
        }

        get method(): crudMethod {
            
            switch (this.$route.params['method']) {
                case 'new':
                    return 'new';

                case 'edit':
                    return 'edit';

                default:
                    return 'invalid';
            }

        }

        get id(): number {
            return Number(this.$route.params['id']);
        }

        get hasWarning() {
            return this.warning !== undefined && this.warning.trim() !== '';
        }

        get firstAnswer() {
            return this.model.answers?.$values[0] ?? new Answer();
        }

        set firstAnswer(value) {
            this.model.answers.$values[0] = value;
        }

        get secondAnswer() {
            return this.model.answers?.$values[1] ?? new Answer();
        }

        set secondAnswer(value) {
            this.model.answers.$values[1] = value;
        }

        get thirdAnswer() {
            return this.model.answers?.$values[2] ?? new Answer();
        }

        set thirdAnswer(value) {
            this.model.answers.$values[2] = value;
        }

        get fourthAnswer() {
            return this.model.answers?.$values[3] ?? new Answer();
        }

        set fourthAnswer(value) {
            this.model.answers.$values[3] = value;
        }

        get categorySelection() {
            return this.category.id;
        }

        set categorySelection(value) {
            this.category = this.categoriesAvailable.find(x => x.id == value) ?? new Category();
        }

        get category() {
            return this.model.category ?? new Category();
        }

        set category(value: Category) {
            this.model.category = value;
        }

        get saveNewButtonText() {
            return this.method == 'new' ? "Create" : "Save";
        }

        private ensureAnswersCreated() {
            if(this.model.answers === undefined) {
                this.model.answers = {
                    $id: "",
                    $values: []
                };
            }
            if(this.model.answers.$values === undefined) {
                this.model.answers.$values = [];
            }

            for (let i = 0; i < 4; i++) {
                if(this.model.answers.$values[i] === undefined) {
                    this.model.answers.$values[i] = new Answer();
                    this.model.answers.$values[i].isCorrect = false;
                }
            }
            this.model.answers.$values[0].isCorrect = true;
        }

        public static async beforeEnter(to: Route, 
                                        from: Route, 
                                        next: NavigationGuardNext<Vue>) {
            let isAuthenticated = false; 
            try {
               isAuthenticated = await AuthApi.IsLoggedIn();
            } catch (error) { }
            if(!isAuthenticated) next('/')
            else next()
        }

    }
</script>

<style scoped>

</style>