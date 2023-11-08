namespace SharedLibrary.Dtos
{
    public class ErrorDto
    {
        public List<string> Errors { get; private set; } //private sadece bu class içinden set edilebilsin dışarıdan başka biri bunları set etmezsin istiyorsa set etmek consracterları kullanacak allatki overload ve normali
        public bool IsShow { get; private set; } //mobil uyg olabilir bu bir apllicationda olabilir sen bu hatayı gösterebilirsin is show true dersem bunu kullanıcıya gösteriyim anlayacak client -false ise kullanıcıya gösterme yazılımcıya gösterecek true ise kullanıcıya

        public ErrorDto()
        {
            Errors = new List<string>();
        }

        public ErrorDto(string error, bool isShow)
        {
            Errors.Add(error);
            isShow = true;
        }

        public ErrorDto(List<string> errors, bool isShow)//overloading
        {
            Errors = Errors;
            IsShow = isShow;
        }

    }

}
